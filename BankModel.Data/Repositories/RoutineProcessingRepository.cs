using BankModel.Data.Interfaces;
using BankModel.Models;
using BankModel.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankModel.Data.Repositories
{
    public class RoutineProcessingRepository : IRoutineProcessingRepository
    {
        private readonly DBContext _context;
        DateTime currentDate;
        public RoutineProcessingRepository(DBContext context)
        {
            _context = context;
            currentDate = Convert.ToDateTime(GetTransactionDate());
        }

        #region

        public bool NoEODPendingItems()
        {
            if (GetPendingSystemUsers() > 0)
                return false;

            if (GetPendingBranch() > 0)
                return false;

            if (GetPendingChartofAccount() > 0)
                return false;

            if (GetPendingProfile() > 0)
                return false;

            if (GetPendingAccount() > 0)
                return false;

            if (GetPendingTransaction() > 0)
                return false;

            if (GetPendingLoan() > 0)
                return false;

            if (GetPendingLoanRepayment() > 0)
                return false;

            if (GetPendingFixedDeposit() > 0)
                return false;

            if (GetPendingSalary() > 0)
                return false;

            if (GetPendingMobileMoney() > 0)
                return false;

            return true;
        }

        public int GetPendingSystemUsers()
        {
            return _context.ApplicationUser.Where(u => u.Status == "PENDING").Count();
        }

        public int GetPendingBranch()
        {
            return _context.Branch.Where(b => b.Status == "PENDING").Count();
        }

        public int GetPendingChartofAccount()
        {
            return _context.ChartOfAccount.Where(c => c.Status == "PENDING").Count();
        }

        public int GetPendingProfile()
        {
            var profileCount = _context.Profile.Where(i => i.Status == "PENDING").Count();
            return (profileCount);
        }

        public int GetPendingAccount()
        {
            return _context.Accounts.Where(a => a.Status == "PENDING").Count();
        }

        public int GetPendingTransaction()
        {
            return _context.Transactions.Where(t => t.Status == "PENDING").Count();
        }

        public int GetPendingLoan()
        {
            return _context.Loan.Where(l => l.Status == "PENDING").Count();
        }

        public int GetPendingLoanRepayment()
        {
            return _context.LoansRepayment.Where(l => l.Status == "PENDING").Count();
        }

        public int GetPendingFixedDeposit()
        {
            return _context.FixedDeposit.Where(f => f.Status == "PENDING").Count();
        }

        public int GetPendingSalary()
        {
            return _context.Transactions.Where(t => t.TransType == "SL" && t.Status == "PENDING").Count();
        }

        public int GetPendingMobileMoney()
        {
            return _context.Transactions.Where(t => t.TransType == "MM" && t.Status == "PENDING").Count(); ;
        }

        public string GetTransactionDate()
        {
            return _context.Parameter.Where(p => p.Name == "TRANSACTION_DATE").Select(p => p.Value).FirstOrDefault();
        }

        #endregion

        //handle end of day processes
        #region
        public async Task<string> EOD(EODViewModel model)
        {
            try
            {
                //Update the status of all users to refelect EOD in progress
                var systemUsers = _context.ApplicationUser.Where(u => u.UserName != model.ActionBy).ToList();
                foreach (var user in systemUsers)
                {
                    user.Status = string.Concat(user.Status, ".EOD");
                }
                await _context.SaveChangesAsync();

                //Complete the following task
                EODSavingsInterst();

                EODOverdrawnAccount();

                UpdateLoansWithNoBalance();

                UpdateDormantLoans();

                TerminateDueLoans();

                UpdateTransactionDate(model.StartDate);

                //Update the status of all users to refelect EOD conclusion
                foreach (var user in systemUsers)
                {
                    var oldStatus = user.Status.Substring(0, user.Status.IndexOf("."));
                    user.Status = oldStatus;
                }
                await _context.SaveChangesAsync();

                return "Succeeded";
            }
            catch (AggregateException ex) { return "Failed" + ex.Message; }
        }

        private void UpdateLoansWithNoBalance()
        {
            var loans = _context.Loan.Where(l => l.LoanBalance == 0).ToList();
            foreach (var loan in loans)
            {
                loan.Status = "COMPLETED";
            }
            _context.SaveChanges();
        }

        private void UpdateDormantLoans()
        {
            var loans = _context.Loan.Where(l => l.LoanBalance != 0 && l.Status == "ACTIVE" && l.TerminationDate <= DateTime.Now.Date).ToList();
            foreach (var loan in loans)
            {
                loan.Status = "DORMANT";
            }
            _context.SaveChanges();
        }

        private void EODSavingsInterst()
        {
            //This retrieves the accounts with interest rate
            var accounts = _context.Accounts.Where(a => a.InterestPerAnnum > 0 && a.BookBalance > 0).ToList();
            foreach (var acc in accounts)
            {
                var account = _context.Accounts.Where(a => a.ID == acc.ID).FirstOrDefault();
                //Get the interest expense account for the branch
                var interestExpenseAccount = _context.ChartOfAccount.Where(c => c.Branch == acc.Branch && c.AccountName == "INTEREST ON SAVINGS")
                    .FirstOrDefault();
                SavingsInterest savings = new SavingsInterest
                {
                    Account = account,
                    Amount = (acc.BookBalance * (acc.InterestPerAnnum / 100)) / 365,
                    BalBF = acc.BookBalance,
                    Rate = acc.InterestPerAnnum,
                    TransDate = currentDate,
                    TransMonth = currentDate.Month.ToString(),
                    TransYear = currentDate.Year.ToString()
                };
                _context.SavingInterest.Add(savings);
                _context.SaveChanges();
            }
        }

        private void EODOverdrawnAccount()
        {
            //This retrieves the accounts with interest rate
            var accounts = _context.Accounts.Where(a => a.ChargeForOverdrawn == true && a.BookBalance < 0).ToList();
            foreach (var acc in accounts)
            {
                decimal amount = 0;
                var account = _context.Accounts.Where(a => a.ID == acc.ID).FirstOrDefault();
                if (acc.OverdrawnChargeType == "FLAT")
                {
                    amount = Convert.ToDecimal(acc.OverdrawnFee);
                }

                if (acc.OverdrawnChargeType == "PERCENT")
                {
                    amount = Math.Abs(acc.BookBalance * (Convert.ToDecimal(acc.OverdrawnFee) / 100));
                }

                OverdrawnAccount overdrawnAccounts = new OverdrawnAccount
                {
                    Account = account,
                    Fee = acc.OverdrawnFee,                 
                    Amount = amount,
                    BalBF = acc.BookBalance,                   
                    FeeType = acc.OverdrawnChargeType,
                    TransDate = currentDate,
                    TransMonth = currentDate.Month.ToString(),
                    TransYear = currentDate.Year.ToString()
                };
                _context.OverdrawnAccount.Add(overdrawnAccounts);
                _context.SaveChanges();
            }
        }

        private void TerminateDueLoans()
        {
            //This terminates loan that are due
            var dueLoans = (from l in _context.Loan
                            join t in _context.TemplateLoan on l.Template.ID equals t.ID
                            where t.AutoTerminate == true && l.TerminationDate <= DateTime.Now.Date && l.Status != "TERMINATED"
                            select l).ToList();

            foreach (var loan in dueLoans)
            {
                //Check if Execute only if funded is set
                if (loan.ExecuteOnlyIfFunded == true)
                {
                    //Get the account balance of the customer and check if it can pay off the loan balance
                    var customerAccount = _context.Accounts.Where(a => a.ID == loan.Account.ID).FirstOrDefault();
                    var customerLiabilityAccount = GetLiabilityAccountNo(customerAccount.ProductCode, customerAccount.Branch.BranchCode);
                    var interestIncome = (from c in _context.ChartOfAccount
                                          join subHead in _context.ChartOfAccountSubHead on c.AccountSubHead.ID equals subHead.ID
                                          where c.AccountName == "INTEREST ON LOAN" && c.Branch.BranchCode == loan.Branch.BranchCode
                                          && subHead.AccountHead == "INCOME" && subHead.AccountName == "INTEREST INCOME"
                                          select c.AccountNo).FirstOrDefault();
                    var refNo = GenerateRefNo();

                    if (customerAccount.BookBalance >= loan.LoanBalance)
                    {
                        //Check if the principal has been paid off
                        if (loan.LoanBalance > loan.TotalInterest)
                        {
                            //Create transaction record for the principal
                            UpdateTransactionHistory(customerLiabilityAccount, loan.LoanAccount.AccountNo, (loan.LoanBalance - loan.TotalInterest),
                                                    string.Concat("PRINCIPAL BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);

                            //Create transaction for the customer statement iro of the principal
                            UpdateTransactionHistory(customerAccount.ID, string.Empty, (loan.LoanBalance - loan.TotalInterest),
                                                    string.Concat("PRINCIPAL BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);

                            //Create transaction for the interest element
                            UpdateTransactionHistory(customerLiabilityAccount, interestIncome, loan.TotalInterest,
                                                    string.Concat("INTEREST BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);

                            //Create transaction for the customer statement iro of the income
                            UpdateTransactionHistory(loan.Account.ID, string.Empty, loan.TotalInterest,
                                                    string.Concat("INTEREST BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);
                        }

                        if (loan.LoanBalance <= loan.TotalInterest)
                        {
                            //Residual balance is only the interest element
                            //Create transaction for the interest element
                            UpdateTransactionHistory(customerLiabilityAccount, interestIncome, loan.LoanBalance,
                                                    string.Concat("INTEREST BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);

                            //Create transaction for the customer statement iro of the income
                            UpdateTransactionHistory(loan.Account.ID, string.Empty, loan.LoanBalance,
                                                    string.Concat("INTEREST BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);
                        }
                    }
                }

                if (loan.ExecuteOnlyIfFunded == false)
                {
                    var customerAccount = _context.Accounts.Where(a => a.ID == loan.Account.ID).FirstOrDefault();
                    var customerLiabilityAccount = GetLiabilityAccountNo(customerAccount.ProductCode, customerAccount.Branch.BranchCode);
                    var interestIncome = (from c in _context.ChartOfAccount
                                          join subHead in _context.ChartOfAccountSubHead on c.AccountSubHead.ID equals subHead.ID
                                          where c.AccountName == "INTEREST ON LOAN" && c.Branch.BranchCode == loan.Branch.BranchCode
                                          && subHead.AccountHead == "INCOME" && subHead.AccountName == "INTEREST INCOME"
                                          select c.AccountNo).FirstOrDefault();
                    var refNo = GenerateRefNo();

                    //Check if the principal has been paid off
                    if (loan.LoanBalance > loan.TotalInterest)
                    {
                        //Create transaction record for the principal
                        UpdateTransactionHistory(customerLiabilityAccount, loan.LoanAccount.AccountNo, (loan.LoanBalance - loan.TotalInterest),
                                                string.Concat("PRINCIPAL BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);

                        //Create transaction for the customer statement iro of the principal
                        UpdateTransactionHistory(customerAccount.ID, string.Empty, (loan.LoanBalance - loan.TotalInterest),
                                                string.Concat("PRINCIPAL BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);

                        //Create transaction for the interest element
                        UpdateTransactionHistory(customerLiabilityAccount, interestIncome, loan.TotalInterest,
                                                string.Concat("INTEREST BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);

                        //Create transaction for the customer statement iro of the income
                        UpdateTransactionHistory(loan.Account.ID, string.Empty, loan.TotalInterest,
                                                string.Concat("INTEREST BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);
                    }

                    if (loan.LoanBalance <= loan.TotalInterest)
                    {
                        //Residual balance is only the interest element
                        //Create transaction for the interest element
                        UpdateTransactionHistory(customerLiabilityAccount, interestIncome, loan.LoanBalance,
                                                string.Concat("INTEREST BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);

                        //Create transaction for the customer statement iro of the income
                        UpdateTransactionHistory(loan.Account.ID, string.Empty, loan.LoanBalance,
                                                string.Concat("INTEREST BALANCE IRO - ", " (", loan.ID + ") TERMINATED"), "LT", refNo);
                    }
                }
            }
        }

        private void TerminateDueFixedDeposit()
        {

        }

        #endregion

        #region
        public string GetProductCodeFromAccountNo(string accountNo)
        {
            //This gets the product code (the 3rd and 4th digit of the account number) from the account no
            return (accountNo.Substring(2, 2));
        }
        private string GetLiabilityAccountNo(string productCode, string branchCode)
        {
            //This gets the name of the product (Liability account) 
            string productName = _context.TemplateAccount.Where(t => t.ProductCode == productCode).Select(t => t.TemplateName).FirstOrDefault();

            return _context.ChartOfAccount.Where(c => c.AccountName == productName && c.Branch.BranchCode == branchCode).Select(c => c.AccountNo).FirstOrDefault();
        }

        private async void UpdateTransactionHistory(string dr, string cr, decimal amount, string narration, string transType, string refNo)
        {
            var currentDate = Convert.ToDateTime(GetTransactionDate());
            var headOfficeBranch = _context.Branch.Where(b => b.BranchCode == "00").FirstOrDefault();

            //Get the balance bf
            decimal drBalBF = 0, drBal = 0, crBalBF = 0, crBal = 0;
            if(dr.Length == 8)
            {
                drBalBF = GetGLAccountBalance(dr);
                UpdateGLAccountBalance(dr, amount, "DR");
                drBal = GetGLAccountBalance(dr);
            }

            if(dr.Length == 10)
            {
                drBalBF = GetCustomerAccountBalance(dr);
                UpdateCustomerAccountBalance(dr, amount, "DR");
                drBal = GetCustomerAccountBalance(dr);
            }

            if (cr.Length == 8)
            {
                crBalBF = GetGLAccountBalance(cr);
                UpdateGLAccountBalance(cr, amount, "CR");
                crBal = GetGLAccountBalance(cr);
            }

            if (cr.Length == 10)
            {
                crBalBF = GetCustomerAccountBalance(cr);
                UpdateCustomerAccountBalance(cr, amount, "CR");
                crBal = GetCustomerAccountBalance(cr);
            }

            Transactions trans = new Transactions
            {
                Amount = amount,
                ApprovedBy = "SYSTEM",
                CR = cr,
                CRBal = crBal,
                CRBalBF = crBalBF,
                DR = dr,
                DRBal = drBal,
                DRBalBF = drBalBF,
                FromBranch = headOfficeBranch,
                InstrumentNo = string.Empty,
                Narration = narration,
                PostedBy = "SYSTEM",
                RefNo = refNo,
                Status = "COMPLETED",
                ToBranch =  headOfficeBranch,
                TransDate = currentDate,
                TransType = transType
            };
            _context.Transactions.Add(trans);
            await _context.SaveChangesAsync();
        }

        private decimal GetCustomerAccountBalance(string account)
        {
            return _context.Accounts.Where(a => a.ID == account).Select(a => a.BookBalance).FirstOrDefault();
        }

        private decimal GetGLAccountBalance(string account)
        {
            return _context.ChartOfAccount.Where(a => a.AccountNo == account).Select(a => a.BookBalance).FirstOrDefault();
        }

        private async void UpdateCustomerAccountBalance(string account, decimal amount, string type)
        {
            var customerAccount = _context.Accounts.Where(a => a.ID == account).FirstOrDefault();
            if (type == "DR")
            {
                customerAccount.BookBalance -= amount;
                await _context.SaveChangesAsync();
            }
            if (type == "CR")
            {
                customerAccount.BookBalance += amount;
                await _context.SaveChangesAsync();
            }

        }

        private async void UpdateGLAccountBalance(string account, decimal amount, string type)
        {
            var glAccount = _context.ChartOfAccount.Where(a => a.AccountNo == account).FirstOrDefault();
            if (type == "DR")
            {
                glAccount.BookBalance -= amount;
                await _context.SaveChangesAsync();
            }
            if (type == "CR")
            {
                glAccount.BookBalance += amount;
                await _context.SaveChangesAsync();
            }
        }

        private void UpdateTransactionDate(DateTime startDate)
        {
            //Update Current Transaction Date
            var currentdate = _context.Parameter.Where(t => t.Name == "TRANSACTION_DATE").FirstOrDefault();
            currentdate.Value = startDate.ToString("yyyy-MM-dd");
            _context.SaveChanges();
        }

        private string GenerateRefNo()
        {
            var currentTransCounter = _context.Parameter.Where(p => p.Name == "TRANSACTION_COUNTER").FirstOrDefault();
            long returnCounter = Convert.ToInt64(currentTransCounter.Value);

            //This updates the customer number counter
            currentTransCounter.Value = (Convert.ToInt64(currentTransCounter.Value) + 1).ToString();
            _context.SaveChanges();

            return (returnCounter.ToString());
        }
        #endregion

        //Handle End of Month processes
        #region
        public bool IsEOMLastDay()
        {
            //This checks if transaction date is the last day of the month
            int lastDay = 0;
            lastDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            if (currentDate.Day == lastDay)
                return true;
            else
                return false;
        }

        public bool EOMLastDayCompleted()
        {
            //Check if all the transaction for the last day in the current transaction month have been captured
            var savingsAccountsToExecute = _context.Accounts.Where(a => a.InterestPerAnnum > 0 && a.BookBalance > 0).Count();

            //Get all the accounts with savings interest computed for the month
            var completedSavingsInterest = _context.SavingInterest.Where(s => Convert.ToInt16(s.TransMonth) == currentDate.Month && Convert.ToInt16(s.TransYear) == currentDate.Year).Count();

            //Check the overdrawn accounts
            var overdrawnAccountsToExecute =  _context.Accounts.Where(a => a.ChargeForOverdrawn == true && a.BookBalance < 0).Count();
            var completedOverdrawnAccounts =  _context.OverdrawnAccount.Where(s => Convert.ToInt16(s.TransMonth) == currentDate.Month && Convert.ToInt16(s.TransYear) == currentDate.Year).Count();

            if (savingsAccountsToExecute == completedSavingsInterest && overdrawnAccountsToExecute == completedOverdrawnAccounts)
                return true;
            else
                return false;
        }

        public bool EOMCompleted()
        {
            //Get all the sms for the month
            return true;
        }

        public string EOMLastDaySavingsInterest(string username)
        {
            try
            {
                EODSavingsInterst();

                //Update the status of all users to refelect EOD in progress
                var systemUsers = _context.ApplicationUser.Where(u => u.UserName != username).ToList();
                foreach (var user in systemUsers)
                {
                    user.Status = string.Concat(user.Status, ".EOD");
                }
                _context.SaveChanges();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public string EOMLastDayOverdrawn()
        {
            try
            {
                EODOverdrawnAccount();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> EOMSavingsInterest()
        {
            try
            {
                //Get all the customers with savings for end of month processing
                var allCustomers = _context.SavingInterest.Where(c => Convert.ToInt16(c.TransMonth) == currentDate.Month && Convert.ToInt16(c.TransYear) == currentDate.Year).Distinct().ToList();
                foreach(var customer in allCustomers)
                {
                    //Compute the total savings of each customer
                    decimal totalSavings = GetCustomerTotalSavings(customer.Account.ID, currentDate.Month, currentDate.Year);
                    
                    //Create the transaction history for the customer 
                    Transactions customerTransaction = new Transactions
                    {
                        Amount = totalSavings,
                        ApprovedBy = "SYSTEM",
                        CR = customer.Account.ID,
                        CRBal = GetCustomerAccountBalance(customer.Account.ID) + totalSavings,
                        CRBalBF = GetCustomerAccountBalance(customer.Account.ID),
                        DR = string.Empty,
                        DRBal = 0,
                        DRBalBF = 0,
                        FromBranch = GetAccountBranch(customer.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "INTEREST ON SAVINGS FOR " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(customer.Account.ID),
                        TransDate = currentDate,
                        TransType = "SI"

                    };
                    _context.Transactions.Add(customerTransaction);

                    //Update the custometr balance
                    UpdateCustomerAccountBalance(customer.Account.ID, totalSavings, "CR");
                    await _context.SaveChangesAsync();

                    //Get the Interest on Savings Expense account
                    var liabilityAccountNo = GetLiabilityAccountNo(GetProductCodeFromAccountNo(customer.Account.ID), GetAccountBranch(customer.Account.ID).BranchCode);
                    var interestExpenseAccount = GetInterestExpenseAccount(GetAccountBranch(customer.Account.ID), liabilityAccountNo);
                    
                    //Create the transaction history for the GL accounts. DR Expense(Interest on Savings) and CR Liability 
                    Transactions glTransaction = new Transactions
                    {
                        Amount = totalSavings,
                        ApprovedBy = "SYSTEM",
                        CR = liabilityAccountNo,
                        CRBal = GetGLAccountBalance(liabilityAccountNo) + totalSavings,
                        CRBalBF = GetGLAccountBalance(liabilityAccountNo),
                        DR = interestExpenseAccount,
                        DRBal = GetGLAccountBalance(interestExpenseAccount) + totalSavings,
                        DRBalBF = GetGLAccountBalance(interestExpenseAccount),
                        FromBranch = GetAccountBranch(customer.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "INTEREST ON SAVINGS FOR " + customer.Account.ID + " " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(customer.Account.ID),
                        TransDate = currentDate,
                        TransType = "SI"

                    };
                    _context.Transactions.Add(glTransaction);

                    //Update the gl account balances
                    UpdateGLAccountBalance(liabilityAccountNo, totalSavings, "CR");
                    UpdateGLAccountBalance(interestExpenseAccount, totalSavings, "DR");

                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> EOMOverdrawnAccount()
        {
            try
            {
                //Get all the customers with overdrawn account for end of month processing
                var allCustomers = _context.OverdrawnAccount.Where(c => Convert.ToInt16(c.TransMonth) == currentDate.Month && Convert.ToInt16(c.TransYear) == currentDate.Year).Distinct().ToList();
                foreach (var customer in allCustomers)
                {
                    //Compute the total savings of each customer
                    decimal totalOverdrawn = GetCustomerTotalOverdrawn(customer.Account.ID, currentDate.Month, currentDate.Year);

                    //Create the transaction history for the customer 
                    Transactions customerTransaction = new Transactions
                    {
                        Amount = totalOverdrawn,
                        ApprovedBy = "SYSTEM",
                        CR = string.Empty,
                        CRBal = 0,
                        CRBalBF = 0,
                        DR = customer.Account.ID,
                        DRBal = GetCustomerAccountBalance(customer.Account.ID) + totalOverdrawn,
                        DRBalBF = GetCustomerAccountBalance(customer.Account.ID),
                        FromBranch = GetAccountBranch(customer.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "CHARGE ON OVERDRAWN ACCOUNT FOR " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(customer.Account.ID),
                        TransDate = currentDate,
                        TransType = "OA"

                    };
                    _context.Transactions.Add(customerTransaction);

                    //Update the custometr balance
                    UpdateCustomerAccountBalance(customer.Account.ID, totalOverdrawn, "DR");
                    await _context.SaveChangesAsync();

                    //Get the Interest on Savings Expense account
                    var liabilityAccountNo = GetLiabilityAccountNo(GetProductCodeFromAccountNo(customer.Account.ID), GetAccountBranch(customer.Account.ID).BranchCode);
                    var interestIncomeAccountNo = GetInterestIncomeAccount(GetAccountBranch(customer.Account.ID), liabilityAccountNo);

                    //Create the transaction history for the GL accounts. DR Expense(Interest on Savings) and CR Liability 
                    Transactions glTransaction = new Transactions
                    {
                        Amount = totalOverdrawn,
                        ApprovedBy = "SYSTEM",
                        CR = interestIncomeAccountNo,
                        CRBal = GetGLAccountBalance(interestIncomeAccountNo) + totalOverdrawn,
                        CRBalBF = GetGLAccountBalance(interestIncomeAccountNo),
                        DR = liabilityAccountNo,
                        DRBal = GetGLAccountBalance(liabilityAccountNo) + totalOverdrawn,
                        DRBalBF = GetGLAccountBalance(liabilityAccountNo),
                        FromBranch = GetAccountBranch(customer.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "CHARGHE FOR OVERDRAWN ON " + customer.Account.ID + " " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(customer.Account.ID),
                        TransDate = currentDate,
                        TransType = "OA"

                    };
                    _context.Transactions.Add(glTransaction);

                    //Update the gl account balances
                    UpdateGLAccountBalance(liabilityAccountNo, totalOverdrawn, "DR");
                    UpdateGLAccountBalance(interestIncomeAccountNo, totalOverdrawn, "CR");

                    await _context.SaveChangesAsync();
                }
                    return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> EOMSMS()
        {
            try
            {
                //Get all the customers sms for end of month processing
                var allCustomers = _context.SMS.Where(c => Convert.ToInt16(c.TransMonth) == currentDate.Month && Convert.ToInt16(c.TransYear) == currentDate.Year).Distinct().ToList();
                foreach (var customer in allCustomers)
                {
                    //Compute the total sms value of each customer
                    decimal totalSMSAmount = GetCustomerTotalSMSAmount(customer.Account.ID, currentDate.Month, currentDate.Year);

                    //Create the transaction history for the customer 
                    Transactions customerTransaction = new Transactions
                    {
                        Amount = totalSMSAmount,
                        ApprovedBy = "SYSTEM",
                        CR = string.Empty,
                        CRBal = 0,
                        CRBalBF = 0,
                        DR = customer.Account.ID,
                        DRBal = GetCustomerAccountBalance(customer.Account.ID) + totalSMSAmount,
                        DRBalBF = GetCustomerAccountBalance(customer.Account.ID),
                        FromBranch = GetAccountBranch(customer.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "SMS CHARGE FOR " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(customer.Account.ID),
                        TransDate = currentDate,
                        TransType = "SMS"

                    };
                    _context.Transactions.Add(customerTransaction);

                    //Update the custometr balance
                    UpdateCustomerAccountBalance(customer.Account.ID, totalSMSAmount, "DR");
                    await _context.SaveChangesAsync();

                    //Get the Interest on Savings Expense account
                    var liabilityAccountNo = GetLiabilityAccountNo(GetProductCodeFromAccountNo(customer.Account.ID), GetAccountBranch(customer.Account.ID).BranchCode);
                    var smsIncomeAccountNo = await GetSMSIncomeAccount(GetAccountBranch(customer.Account.ID));

                    //Create the transaction history for the GL accounts. DR Expense(Interest on Savings) and CR Liability 
                    Transactions glTransaction = new Transactions
                    {
                        Amount = totalSMSAmount,
                        ApprovedBy = "SYSTEM",
                        CR = smsIncomeAccountNo,
                        CRBal = GetGLAccountBalance(smsIncomeAccountNo) + totalSMSAmount,
                        CRBalBF = GetGLAccountBalance(smsIncomeAccountNo),
                        DR = liabilityAccountNo,
                        DRBal = GetGLAccountBalance(liabilityAccountNo) + totalSMSAmount,
                        DRBalBF = GetGLAccountBalance(liabilityAccountNo),
                        FromBranch = GetAccountBranch(customer.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "SMS CHARGE ON " + customer.Account.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(customer.Account.ID),
                        TransDate = currentDate,
                        TransType = "SMS"

                    };
                    _context.Transactions.Add(glTransaction);

                    //Update the gl account balances
                    UpdateGLAccountBalance(liabilityAccountNo, totalSMSAmount, "DR");
                    UpdateGLAccountBalance(smsIncomeAccountNo, totalSMSAmount, "CR");

                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> EOMLoanRepayment()
        {
            try
            {
                //Get all the customers sms for end of month processing
                var allLoans = _context.LoansRepayment.Where(c => Convert.ToInt16(c.RepaymentMonth) == currentDate.Month && Convert.ToInt16(c.RepaymentYear) == currentDate.Year).Include(a => a.Account).Include(b => b.Branch).ToList();
                foreach (var loan in allLoans)
                {
                    //Get the loan of the customer
                    var customerLoan = _context.Loan.Where(l => l.ID == loan.Loan.ID).FirstOrDefault();

                    //Create the transaction history for the customer for the principal
                    Transactions principalTransaction = new Transactions
                    {
                        Amount = loan.Principal,
                        ApprovedBy = "SYSTEM",
                        CR = string.Empty,
                        CRBal = 0,
                        CRBalBF = 0,
                        DR = loan.Account.ID,
                        DRBal = GetCustomerAccountBalance(loan.Account.ID) + loan.Principal,
                        DRBalBF = GetCustomerAccountBalance(loan.Account.ID),
                        FromBranch = GetAccountBranch(loan.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "PRINCIPAL REPAYMENT FOR LOAN " + loan.Loan.ID + " " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(loan.Account.ID),
                        TransDate = currentDate,
                        TransType = "LR"

                    };
                    _context.Transactions.Add(principalTransaction);

                    //Update the custometr balance with the principal
                    UpdateCustomerAccountBalance(loan.Account.ID, loan.Principal, "DR");
                    customerLoan.LoanBalance -= loan.Principal;
                    await _context.SaveChangesAsync();

                    //Create the transaction for interest element of the loan
                    Transactions interestTransaction = new Transactions
                    {
                        Amount = loan.Interest,
                        ApprovedBy = "SYSTEM",
                        CR = string.Empty,
                        CRBal = 0,
                        CRBalBF = 0,
                        DR = loan.Account.ID,
                        DRBal = GetCustomerAccountBalance(loan.Account.ID) + loan.Interest,
                        DRBalBF = GetCustomerAccountBalance(loan.Account.ID),
                        FromBranch = GetAccountBranch(loan.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "INTEREST REPAYMENT FOR LOAN " + loan.Loan.ID + " " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(loan.Account.ID),
                        TransDate = currentDate,
                        TransType = "LR"

                    };
                    _context.Transactions.Add(interestTransaction);

                    //Update the custometr balance with the principal
                    UpdateCustomerAccountBalance(loan.Account.ID, loan.Interest, "DR");
                    customerLoan.LoanBalance -= loan.Interest;
                    customerLoan.TotalInterest -= loan.Interest;
                    await _context.SaveChangesAsync();

                    //Get the customer liability account and loan
                    var liabilityAccountNo = GetLiabilityAccountNo(GetProductCodeFromAccountNo(loan.Account.ID), GetAccountBranch(loan.Account.ID).BranchCode);

                    //Create the transaction history for the GL accounts. DR Liability and CR Interest on loan 
                    Transactions glPrincipalTransaction = new Transactions
                    {
                        Amount = loan.Principal,
                        ApprovedBy = "SYSTEM",
                        CR = customerLoan.LoanAccount.AccountNo,
                        CRBal = GetGLAccountBalance(customerLoan.LoanAccount.AccountNo) + loan.Principal,
                        CRBalBF = GetGLAccountBalance(customerLoan.LoanAccount.AccountNo),
                        DR = liabilityAccountNo,
                        DRBal = GetGLAccountBalance(liabilityAccountNo) + loan.Principal,
                        DRBalBF = GetGLAccountBalance(liabilityAccountNo),
                        FromBranch = GetAccountBranch(loan.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "PRINCIPAL REPAYMENT ON " + loan.Account.ID + " FOR LOAN " + loan.Loan.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(loan.Account.ID),
                        TransDate = currentDate,
                        TransType = "LR"

                    };
                    _context.Transactions.Add(glPrincipalTransaction);

                    //Update the gl account balances
                    UpdateGLAccountBalance(liabilityAccountNo, loan.Principal, "DR");
                    UpdateGLAccountBalance(customerLoan.LoanAccount.AccountNo, loan.Principal, "CR");

                    await _context.SaveChangesAsync();

                    var loanInterestAccount = GetLoanInterestAccount(loan.Branch, liabilityAccountNo);
                    Transactions glInterestTransaction = new Transactions
                    {
                        Amount = loan.Interest,
                        ApprovedBy = "SYSTEM",
                        CR = loanInterestAccount,
                        CRBal = GetGLAccountBalance(loanInterestAccount) + loan.Interest,
                        CRBalBF = GetGLAccountBalance(loanInterestAccount),
                        DR = liabilityAccountNo,
                        DRBal = GetGLAccountBalance(liabilityAccountNo) + loan.Interest,
                        DRBalBF = GetGLAccountBalance(liabilityAccountNo),
                        FromBranch = GetAccountBranch(loan.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = "INTEREST REPAYMENT ON " + loan.Account.ID + " FOR LOAN " + loan.Loan.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(loan.Account.ID),
                        TransDate = currentDate,
                        TransType = "LR"

                    };
                    _context.Transactions.Add(glInterestTransaction);

                    //Update the gl account balances
                    UpdateGLAccountBalance(liabilityAccountNo, loan.Interest, "DR");
                    UpdateGLAccountBalance(loanInterestAccount, loan.Interest, "CR");

                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> EOMLoanDefault()
        {
            try
            {
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> EOMFixedDeposit()
        {
            try
            {
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> EOMStandingOrder()
        {
            try
            {
                //Get all the standing orders to be processed annually
                if (currentDate.Month == 12)
                {
                    var annualStandingOrders = _context.StandingOrder.Where(s => s.Fees.Frequency == "ANNUALLY").Include(f => f.Fees).ToList();
                    foreach (var order in annualStandingOrders)
                    {
                        //Create the transaction history for the customer statemet
                        Transactions transaction = new Transactions
                        {
                            Amount = order.Fees.Amount,
                            ApprovedBy = "SYSTEM",
                            CR = string.Empty,
                            CRBal = 0,
                            CRBalBF = 0,
                            DR = order.Account.ID,
                            DRBal = GetCustomerAccountBalance(order.Account.ID) + order.Fees.Amount,
                            DRBalBF = GetCustomerAccountBalance(order.Account.ID),
                            FromBranch = GetAccountBranch(order.Account.ID),
                            InstrumentNo = string.Empty,
                            Narration = order.Fees.Narration +  " ON " + order.Account.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                            PostedBy = "SYSTEM",
                            RefNo = string.Empty,
                            Status = "ACTIVE",
                            ToBranch = GetAccountBranch(order.Account.ID),
                            TransDate = currentDate,
                            TransType = "SO"

                        };
                        _context.Transactions.Add(transaction);

                        //Update the custometr balance
                        UpdateCustomerAccountBalance(order.Account.ID, order.Fees.Amount, "DR");
                        await _context.SaveChangesAsync();

                        //Create the transaction history for the liability account
                        var liabilityAccountNo = GetLiabilityAccountNo(GetProductCodeFromAccountNo(order.Account.ID), GetAccountBranch(order.Account.ID).BranchCode);
                        Transactions glTransaction = new Transactions
                        {
                            Amount = order.Fees.Amount,
                            ApprovedBy = "SYSTEM",
                            CR = order.Fees.IncomeAccount,
                            CRBal = GetGLAccountBalance(order.Fees.IncomeAccount) + order.Fees.Amount,
                            CRBalBF = GetGLAccountBalance(order.Fees.IncomeAccount),
                            DR = liabilityAccountNo,
                            DRBal = GetGLAccountBalance(liabilityAccountNo) + order.Fees.Amount,
                            DRBalBF = GetGLAccountBalance(liabilityAccountNo),
                            FromBranch = GetAccountBranch(order.Account.ID),
                            InstrumentNo = string.Empty,
                            Narration = order.Fees.Narration + "ON " + order.Account.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                            PostedBy = "SYSTEM",
                            RefNo = string.Empty,
                            Status = "ACTIVE",
                            ToBranch = GetAccountBranch(order.Account.ID),
                            TransDate = currentDate,
                            TransType = "SO"

                        };
                        _context.Transactions.Add(glTransaction);

                        //Update the gl account balances
                        UpdateGLAccountBalance(liabilityAccountNo, order.Fees.Amount, "DR");
                        UpdateGLAccountBalance(order.Fees.IncomeAccount, order.Fees.Amount, "CR");
                        await _context.SaveChangesAsync();
                    }
                }

                //Get all the standing orders to be processed bi annually
                if (currentDate.Month == 6 || currentDate.Month == 12)
                {
                    var biAnnualStandingOrders = _context.StandingOrder.Where(s => s.Fees.Frequency == "BI-ANNUAL").Include(f => f.Fees).ToList();
                    foreach (var order in biAnnualStandingOrders)
                    {
                        //Create the transaction history for the customer statemet
                        Transactions transaction = new Transactions
                        {
                            Amount = order.Fees.Amount,
                            ApprovedBy = "SYSTEM",
                            CR = string.Empty,
                            CRBal = 0,
                            CRBalBF = 0,
                            DR = order.Account.ID,
                            DRBal = GetCustomerAccountBalance(order.Account.ID) + order.Fees.Amount,
                            DRBalBF = GetCustomerAccountBalance(order.Account.ID),
                            FromBranch = GetAccountBranch(order.Account.ID),
                            InstrumentNo = string.Empty,
                            Narration = order.Fees.Narration + " ON " + order.Account.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                            PostedBy = "SYSTEM",
                            RefNo = string.Empty,
                            Status = "ACTIVE",
                            ToBranch = GetAccountBranch(order.Account.ID),
                            TransDate = currentDate,
                            TransType = "SO"

                        };
                        _context.Transactions.Add(transaction);

                        //Update the custometr balance
                        UpdateCustomerAccountBalance(order.Account.ID, order.Fees.Amount, "DR");
                        await _context.SaveChangesAsync();

                        //Create the transaction history for the liability account
                        var liabilityAccountNo = GetLiabilityAccountNo(GetProductCodeFromAccountNo(order.Account.ID), GetAccountBranch(order.Account.ID).BranchCode);
                        Transactions glTransaction = new Transactions
                        {
                            Amount = order.Fees.Amount,
                            ApprovedBy = "SYSTEM",
                            CR = order.Fees.IncomeAccount,
                            CRBal = GetGLAccountBalance(order.Fees.IncomeAccount) + order.Fees.Amount,
                            CRBalBF = GetGLAccountBalance(order.Fees.IncomeAccount),
                            DR = liabilityAccountNo,
                            DRBal = GetGLAccountBalance(liabilityAccountNo) + order.Fees.Amount,
                            DRBalBF = GetGLAccountBalance(liabilityAccountNo),
                            FromBranch = GetAccountBranch(order.Account.ID),
                            InstrumentNo = string.Empty,
                            Narration = order.Fees.Narration + "ON " + order.Account.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                            PostedBy = "SYSTEM",
                            RefNo = string.Empty,
                            Status = "ACTIVE",
                            ToBranch = GetAccountBranch(order.Account.ID),
                            TransDate = currentDate,
                            TransType = "SO"

                        };
                        _context.Transactions.Add(glTransaction);

                        //Update the gl account balances
                        UpdateGLAccountBalance(liabilityAccountNo, order.Fees.Amount, "DR");
                        UpdateGLAccountBalance(order.Fees.IncomeAccount, order.Fees.Amount, "CR");
                        await _context.SaveChangesAsync();
                    }
                }


                //Get all the standing orders to be processed quarterly
                if (currentDate.Month == 3 || currentDate.Month == 6 || currentDate.Month == 9 || currentDate.Month == 12)
                {
                    var quarterlyStandingOrders = _context.StandingOrder.Where(s => s.Fees.Frequency == "QUARTERLY").Include(f => f.Fees).ToList();
                    foreach(var order in quarterlyStandingOrders)
                    {
                        //Create the transaction history for the customer statemet
                        Transactions transaction = new Transactions
                        {
                            Amount = order.Fees.Amount,
                            ApprovedBy = "SYSTEM",
                            CR = string.Empty,
                            CRBal = 0,
                            CRBalBF = 0,
                            DR = order.Account.ID,
                            DRBal = GetCustomerAccountBalance(order.Account.ID) + order.Fees.Amount,
                            DRBalBF = GetCustomerAccountBalance(order.Account.ID),
                            FromBranch = GetAccountBranch(order.Account.ID),
                            InstrumentNo = string.Empty,
                            Narration = order.Fees.Narration + " ON " + order.Account.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                            PostedBy = "SYSTEM",
                            RefNo = string.Empty,
                            Status = "ACTIVE",
                            ToBranch = GetAccountBranch(order.Account.ID),
                            TransDate = currentDate,
                            TransType = "SO"

                        };
                        _context.Transactions.Add(transaction);

                        //Update the custometr balance
                        UpdateCustomerAccountBalance(order.Account.ID, order.Fees.Amount, "DR");
                        await _context.SaveChangesAsync();

                        //Create the transaction history for the liability account
                        var liabilityAccountNo = GetLiabilityAccountNo(GetProductCodeFromAccountNo(order.Account.ID), GetAccountBranch(order.Account.ID).BranchCode);
                        Transactions glTransaction = new Transactions
                        {
                            Amount = order.Fees.Amount,
                            ApprovedBy = "SYSTEM",
                            CR = order.Fees.IncomeAccount,
                            CRBal = GetGLAccountBalance(order.Fees.IncomeAccount) + order.Fees.Amount,
                            CRBalBF = GetGLAccountBalance(order.Fees.IncomeAccount),
                            DR = liabilityAccountNo,
                            DRBal = GetGLAccountBalance(liabilityAccountNo) + order.Fees.Amount,
                            DRBalBF = GetGLAccountBalance(liabilityAccountNo),
                            FromBranch = GetAccountBranch(order.Account.ID),
                            InstrumentNo = string.Empty,
                            Narration = order.Fees.Narration + "ON " + order.Account.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                            PostedBy = "SYSTEM",
                            RefNo = string.Empty,
                            Status = "ACTIVE",
                            ToBranch = GetAccountBranch(order.Account.ID),
                            TransDate = currentDate,
                            TransType = "SO"

                        };
                        _context.Transactions.Add(glTransaction);

                        //Update the gl account balances
                        UpdateGLAccountBalance(liabilityAccountNo, order.Fees.Amount, "DR");
                        UpdateGLAccountBalance(order.Fees.IncomeAccount, order.Fees.Amount, "CR");
                        await _context.SaveChangesAsync();
                    }
                }

                //Get all the monthly standing orders to be processed
                var monthlyStandingOrder = _context.StandingOrder.Where(s => s.Fees.Frequency == "MONTHLY").Include(f => f.Fees).ToList();
                foreach(var order in monthlyStandingOrder)
                {
                    //Create the transaction history for the customer statemet
                    Transactions transaction = new Transactions
                    {
                        Amount = order.Fees.Amount,
                        ApprovedBy = "SYSTEM",
                        CR = string.Empty,
                        CRBal = 0,
                        CRBalBF = 0,
                        DR = order.Account.ID,
                        DRBal = GetCustomerAccountBalance(order.Account.ID) + order.Fees.Amount,
                        DRBalBF = GetCustomerAccountBalance(order.Account.ID),
                        FromBranch = GetAccountBranch(order.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = order.Fees.Narration + " ON " + order.Account.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(order.Account.ID),
                        TransDate = currentDate,
                        TransType = "SO"

                    };
                    _context.Transactions.Add(transaction);

                    //Update the custometr balance
                    UpdateCustomerAccountBalance(order.Account.ID, order.Fees.Amount, "DR");
                    await _context.SaveChangesAsync();

                    //Create the transaction history for the liability account
                    var liabilityAccountNo = GetLiabilityAccountNo(GetProductCodeFromAccountNo(order.Account.ID), GetAccountBranch(order.Account.ID).BranchCode);
                    Transactions glTransaction = new Transactions
                    {
                        Amount = order.Fees.Amount,
                        ApprovedBy = "SYSTEM",
                        CR = order.Fees.IncomeAccount,
                        CRBal = GetGLAccountBalance(order.Fees.IncomeAccount) + order.Fees.Amount,
                        CRBalBF = GetGLAccountBalance(order.Fees.IncomeAccount),
                        DR = liabilityAccountNo,
                        DRBal = GetGLAccountBalance(liabilityAccountNo) + order.Fees.Amount,
                        DRBalBF = GetGLAccountBalance(liabilityAccountNo),
                        FromBranch = GetAccountBranch(order.Account.ID),
                        InstrumentNo = string.Empty,
                        Narration = order.Fees.Narration + "ON " + order.Account.ID + " FOR " + currentDate.Month + "/" + currentDate.Year,
                        PostedBy = "SYSTEM",
                        RefNo = string.Empty,
                        Status = "ACTIVE",
                        ToBranch = GetAccountBranch(order.Account.ID),
                        TransDate = currentDate,
                        TransType = "SO"

                    };
                    _context.Transactions.Add(glTransaction);

                    //Update the gl account balances
                    UpdateGLAccountBalance(liabilityAccountNo, order.Fees.Amount, "DR");
                    UpdateGLAccountBalance(order.Fees.IncomeAccount, order.Fees.Amount, "CR");
                    await _context.SaveChangesAsync();
                }

                var standingOrders = _context.StandingOrder.ToList();
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> EOMProfitandLoss()
        {
            int transMonth = 0, transYear = 0;
            if(currentDate.Month == 1)
            {
                transMonth = 12;
                transYear = currentDate.Year - 1;
            }

            if (currentDate.Month != 1)
            {
                transMonth = currentDate.Month - 1;
                transYear = currentDate.Year;
            }

            try
            {
                //Get all the branches
                var branches = _context.Branch.ToList();
                foreach(Branch branch in branches)
                {
                    //Get all the P & L items
                    var plItems = _context.ChartOfAccount.Where(c => c.AccountSubHead.AccountHead == "INCOME" || c.AccountSubHead.AccountHead == "EXPENSE"
                                                                && c.Branch == branch)
                                                                .Include(c => c.AccountSubHead)
                                                                .Include(c => c.Branch).ToList();
                    foreach(var item in plItems)
                    {
                        decimal drBalanceBF = 0, crBalanceBF = 0;
                        //Get the last month and year transaction
                        var lastPLTransaction = _context.ProfitAndLoss.Where(t => t.AccountNo == item.AccountNo && t.TransMonth == transMonth && t.TransYear == transYear).FirstOrDefault();
                        if(lastPLTransaction == null)
                        {
                            drBalanceBF = 0;
                            crBalanceBF = 0;
                        }

                        if(lastPLTransaction != null)
                        {
                            drBalanceBF = lastPLTransaction.ClosingDRBal;
                            crBalanceBF = lastPLTransaction.ClosingCRBal;
                        }

                        //Get all the debit transactions of the PL item
                        var currentDR = _context.Transactions.Where(t => t.DR == item.AccountNo).Select(t => t.Amount).Sum();
                        var currentCR = _context.Transactions.Where(t => t.CR == item.AccountNo).Select(t => t.Amount).Sum();

                        ProfitAndLoss pl = new ProfitAndLoss
                        {
                            AccountName = item.AccountName,
                            AccountNo = item.AccountNo,
                            AccountSubType = item.AccountSubHead.AccountName,
                            AccountType = item.AccountSubHead.AccountHead,
                            Branch = branch,
                            ClosingCRBal = crBalanceBF + currentCR,
                            ClosingDRBal = drBalanceBF + currentDR,
                            CRBalBF = crBalanceBF,
                            CurrentCR = currentCR,
                            CurrentDR = currentDR,
                            DRBalBF = drBalanceBF,
                            TransYear = currentDate.Year,
                            TransMonth = currentDate.Month,
                            TransDate = currentDate,

                        };

                        _context.ProfitAndLoss.Add(pl);
                        await _context.SaveChangesAsync();
                    }
                }

                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> EOMBalanceSheet()
        {
            int transMonth = 0, transYear = 0;
            decimal lastFinYearBalance = 0, lastTwoFinYearBalance = 0, lastMonthBalance = 0;
            if (currentDate.Month == 1)
            {
                transMonth = 12;
                transYear = currentDate.Year - 1;
            }

            if(currentDate.Month != 1)
            {
                transMonth = currentDate.Month -1 ;
                transYear = currentDate.Year;
            }

            try
            {
                var branches = _context.Branch.ToList();
                foreach(Branch branch in branches)
                {
                    //Get all the P & L items
                    var balanceSheetItems = _context.ChartOfAccount.Where(c => c.AccountSubHead.AccountHead == "ASSET" || c.AccountSubHead.AccountHead == "LIABILITY"
                                                                        && c.Branch == branch).Include(c => c.AccountSubHead)
                                                                        .Include(a => a.AccountSubHead)
                                                                        .Include(b => b.Branch).ToList();
                    foreach(var item in balanceSheetItems)
                    {
                        //Get the last financial year balance
                        var lastYearBalanceSheet = _context.BalanceSheet.Where(b => b.AccountCode == item.AccountSubHead.AccountCode
                                                                            && b.AccountType == item.AccountSubHead.AccountHead
                                                                            && b.AccountSubType == item.AccountSubHead.AccountName
                                                                            && b.AccountDescription == item.AccountName
                                                                            && b.TransMonth == 12 && b.TransYear == (currentDate.Year - 1)).FirstOrDefault();

                        if(lastYearBalanceSheet == null)
                        {
                            lastFinYearBalance = 0;
                        }
                        else
                        {
                            lastFinYearBalance = lastYearBalanceSheet.CurrentBal;
                        }

                        //Get the last two financial year balance
                        var lastTwoYearBalanceSheet = _context.BalanceSheet.Where(b => b.AccountCode == item.AccountSubHead.AccountCode
                                                                            && b.AccountType == item.AccountSubHead.AccountHead
                                                                            && b.AccountSubType == item.AccountSubHead.AccountName
                                                                            && b.AccountDescription == item.AccountName
                                                                            && b.TransMonth == 12 && b.TransYear == (currentDate.Year - 2)).FirstOrDefault();

                        if (lastTwoYearBalanceSheet == null)
                        {
                            lastTwoFinYearBalance = 0;
                        }
                        else
                        {
                            lastTwoFinYearBalance = lastTwoYearBalanceSheet.CurrentBal;
                        }

                        //Get the last month balannce
                        var lastMonthBalanceSheet = _context.BalanceSheet.Where(b => b.AccountCode == item.AccountSubHead.AccountCode
                                                                            && b.AccountType == item.AccountSubHead.AccountHead
                                                                            && b.AccountSubType == item.AccountSubHead.AccountName
                                                                            && b.AccountDescription == item.AccountName
                                                                            && b.TransMonth == transMonth
                                                                            && b.TransYear == transYear).FirstOrDefault();

                        if (lastMonthBalanceSheet == null)
                        {
                            lastMonthBalance = 0;
                        }
                        else
                        {
                            lastMonthBalance = lastMonthBalanceSheet.CurrentBal;
                        }

                        BalanceSheet balanceSheet = new BalanceSheet
                        {
                            AccountCode = item.AccountSubHead.AccountCode,
                            AccountDescription = item.AccountName,
                            AccountSubType = item.AccountSubHead.AccountName,
                            AccountType = item.AccountSubHead.AccountHead,
                            Branch = branch,
                            CurrentBal = item.BookBalance,
                            LastFinancialYearBal = lastFinYearBalance,
                            LastTwoFinancialYearBal = lastTwoFinYearBalance,
                            LastMonthBal = lastMonthBalance,
                            TransMonth = currentDate.Month,
                            TransYear = currentDate.Year,
                            TransDate = currentDate
                        };
                        _context.BalanceSheet.Add(balanceSheet);
                        await _context.SaveChangesAsync();
                    }
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> StartofMonth(EODViewModel model)
        {
            try
            {

                //Update the status of all users to refelect EOD conclusion
                var systemUsers = _context.ApplicationUser.Where(u => u.UserName != model.ActionBy).ToList();
                foreach (var user in systemUsers)
                {
                    var oldStatus = user.Status.Substring(0, user.Status.IndexOf("."));
                    user.Status = oldStatus;
                }
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        private decimal GetCustomerTotalSavings(string accountNo, int month, int year)
        {
            return _context.SavingInterest.Where(c => Convert.ToInt16(c.TransMonth) == currentDate.Month
                                                && Convert.ToInt16(c.TransYear) == currentDate.Year
                                                && c.Account.ID == accountNo).Select(c => c.Amount).Sum();
        }

        private decimal GetCustomerTotalOverdrawn(string accountNo, int month, int year)
        {
            return _context.OverdrawnAccount.Where(c => Convert.ToInt16(c.TransMonth) == currentDate.Month
                                                && Convert.ToInt16(c.TransYear) == currentDate.Year
                                                && c.Account.ID == accountNo).Select(c => c.Amount).Sum();
        }

        private decimal GetCustomerTotalSMSAmount(string accountNo, int month, int year)
        {
            return _context.SMS.Where(c => Convert.ToInt16(c.TransMonth) == currentDate.Month
                                                && Convert.ToInt16(c.TransYear) == currentDate.Year
                                                && c.Account.ID == accountNo).Select(c => c.Amount).Sum();
        }

        private string GetInterestExpenseAccount(Branch branch, string liabilityAccountNo)
        {
            var liabilityAccountName = _context.ChartOfAccount.Where(c => c.AccountNo == liabilityAccountNo).Select(c => c.AccountName).FirstOrDefault();
            return _context.ChartOfAccount.Where(c => c.Branch == branch && c.AccountName == liabilityAccountName
            && c.AccountSubHead.AccountHead == "EXPENSE" && c.AccountSubHead.AccountName == "INTEREST EXPENSE").Select(c => c.AccountNo).FirstOrDefault();
        }

        private string GetInterestIncomeAccount(Branch branch, string liabilityAccountNo)
        {
            var liabilityAccountName = _context.ChartOfAccount.Where(c => c.AccountNo == liabilityAccountNo).Select(c => c.AccountName).FirstOrDefault();
            return _context.ChartOfAccount.Where(c => c.Branch == branch && c.AccountName == liabilityAccountName
            && c.AccountSubHead.AccountHead == "INCOME" && c.AccountSubHead.AccountName == "INTEREST INCOME").Select(c => c.AccountNo).FirstOrDefault();
        }

        private string GetLoanAccount(Branch branch, string loanType)
        {
            return _context.ChartOfAccount.Where(c => c.AccountName == loanType && c.Branch == branch).Select(c => c.AccountNo).FirstOrDefault();
        }

        private string GetLoanInterestAccount(Branch branch, string liabilityAccountNo)
        {
            var liabilityAccountName = _context.ChartOfAccount.Where(c => c.AccountNo == liabilityAccountNo).Select(c => c.AccountName).FirstOrDefault();
            return _context.ChartOfAccount.Where(c => c.Branch == branch && c.AccountName == liabilityAccountName
            && c.AccountSubHead.AccountHead == "INCOME" && c.AccountSubHead.AccountName == "INTEREST INCOME").Select(c => c.AccountNo).FirstOrDefault();
        }

        private string GetLiabilityAccountNameFromNo(string liabilityAccountNo)
        {
            return _context.ChartOfAccount.Where(c => c.AccountNo == liabilityAccountNo).Select(c => c.AccountName).FirstOrDefault();
        }

        private Branch GetAccountBranch(string accountNo)
        {
            return _context.Accounts.Where(a => a.ID == accountNo).Select(b => b.Branch).FirstOrDefault();
        }

        private async Task<string> GetSMSIncomeAccount(Branch branch)
        {
            var smsIncomeAccount = _context.ChartOfAccount.Where(c => c.Branch == branch && c.AccountName == "SMS INCOME" 
                                                                && c.AccountSubHead.AccountHead == "INCOME" 
                                                                && c.AccountSubHead.AccountName == "OTHER INCOME").FirstOrDefault();
            if(smsIncomeAccount == null)
            {
                //Create the SMS income account
                var accountSubHead = _context.ChartOfAccountSubHead.Where(c => c.AccountHead == "EXPENSE" && c.AccountName == "INTEREST EXPENSE").FirstOrDefault();
                ChartOfAccount smsAccount = new ChartOfAccount
                {
                    AccountName = "SMS INCOME",
                    AccountNo = GenerateChartofAccountNumber(accountSubHead.AccountHead, accountSubHead.AccountName, branch.BranchDesc),
                    AccountSubHead = accountSubHead,
                    ApprovedBy = "SYSTEM",
                    BookBalance = 0,
                    Branch = branch,
                    PostedBy = "SYSTEM",
                    Status = "ACTIVE",
                    TransDate = currentDate
                };
                _context.ChartOfAccount.Add(smsAccount);
                await _context.SaveChangesAsync();

                var newSMSIncomeAccount = _context.ChartOfAccount.Where(c => c.Branch == branch && c.AccountName == "SMS INCOME"
                                                     && c.AccountSubHead.AccountHead == "INCOME"
                                                     && c.AccountSubHead.AccountName == "OTHER INCOME").FirstOrDefault();
                return newSMSIncomeAccount.AccountNo;
            }

            return smsIncomeAccount.AccountNo;
        }

        private string GenerateChartofAccountNumber(string accountHead, string subHead, string branchDesc)
        {
            //This gets the branch code for the transaction
            string branchCode = GetBranchCodeFromName(branchDesc);

            //This assigns values to the account head (Asset, Liability, Income and Expense)
            string accountHeadCode = string.Empty;
            if (accountHead == "ASSET")
                accountHeadCode = "01";
            else if (accountHead == "LIABILITY")
                accountHeadCode = "02";
            else if (accountHead == "INCOME")
                accountHeadCode = "03";
            else if (accountHead == "EXPENSE")
                accountHeadCode = "04";

            var accountSubHeadCode = _context.ChartOfAccountSubHead.Where(a => a.AccountHead == accountHead && a.AccountName == subHead).Select(a => a.AccountCode).FirstOrDefault();

            string chartofAccountCounter = GenerateChartofAccountCounter(branchCode).ToString();
            if (chartofAccountCounter.Length < 2)
                chartofAccountCounter = string.Concat("0", chartofAccountCounter);

            //This generates the account no in the formant (BranchCode AccountHead AccountSubHead UniqueCounter - 00011119)
            string chartofAccountNo = string.Concat(branchCode, accountHeadCode, accountSubHeadCode, chartofAccountCounter);
            return (chartofAccountNo);
        }

        public string GetBranchCodeFromName(string branchName)
        {
            return _context.Branch.Where(b => b.BranchDesc == branchName).Select(b => b.BranchCode).FirstOrDefault();
        }

        private int GenerateChartofAccountCounter(string branchCode)
        {
            //This gets the current counter
            int currentCounter = 0;
            var branch = _context.Branch.Where(b => b.BranchCode == branchCode).FirstOrDefault();

            if (branch == null)
            {
                currentCounter = 1;
            }
            else if (branch != null)
            {
                currentCounter = branch.ChartofAccountCounter;
                //This updates the counter by adding 1
                branch.ChartofAccountCounter = currentCounter + 1;
                _context.SaveChanges();
            }

            return (currentCounter);
        }

        #endregion

        #region
        //Handle End of Year
        public async Task<string> EOY(EODViewModel model)
        {
            await _context.SaveChangesAsync();
            return "Succeeded";
        }

        #endregion



    }
}
