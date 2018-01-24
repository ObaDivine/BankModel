using BankModel.Data.Interfaces;
using BankModel.Web;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;
using BankModel.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BankModel.Data.Repositories
{
    public class CashRepository : ICashRepository
    {
        private readonly DBContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public CashRepository(DBContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #region
        public string GetBranchCodeFromAccountNo(string accountNo)
        {
            //This gets the branch code (the 1st and 2nd digits) from the account number
            return (accountNo.Substring(0, 2));
        }

        public bool InterBranchAccountExist(string debitAccount, string creditAccount)
        {
            bool interBranchAccountExist = false;
            string drBranchCode = GetBranchCodeFromAccountNo(debitAccount);
            string crBranchCode = GetBranchCodeFromAccountNo(creditAccount);

            //This gets the branch description using the branch code
            string branchDesc = _context.Branch.Where(b => b.BranchCode == crBranchCode).Select(b => b.BranchDesc).FirstOrDefault();
            var interBranch = _context.ChartOfAccount.Where(c => c.Branch.BranchCode == drBranchCode && c.AccountName == string.Concat("INTERBRANCH ", branchDesc))
                .Include(b => b.Branch)
                .FirstOrDefault();

            if (interBranch == null ? interBranchAccountExist = false : interBranchAccountExist = true) {;}

            return (interBranchAccountExist);
        }

        public string GetProductCodeFromAccountNo(string accountNo)
        {
            //This gets the product code (the 3rd and 4th digit of the account number) from the account no
            return (accountNo.Substring(2, 2));
        }

        public string GetLiabilityAccountNo(string productCode, string branchCode)
        {
            //This gets the name of the product (Liability account) 
            string productName = _context.TemplateAccount.Where(t => t.ProductCode == productCode).Select(t => t.TemplateName).FirstOrDefault();

            return _context.ChartOfAccount.Where(c => c.AccountName == productName && c.Branch.BranchCode == branchCode)
                .Include(b => b.Branch)
                .Select(c => c.AccountNo)
                .FirstOrDefault();
        }

        public Account CheckIfAccountExist(string accountNo)
        {
            return _context.Accounts.Where(a => a.ID == accountNo).FirstOrDefault();
        }

        public bool CheckIfAccountIsFunded(string transType, string accountNo, decimal amount)
        {
            bool accountFunded = false;
            decimal pendingTransAmount = 0;
            if (transType == "CUSTOMER")
            {
                var accountDetails = _context.Accounts.Where(a => a.ID == accountNo).FirstOrDefault();

                //This gets the list of pending transactions
                var pendingTrans = _context.Transactions.Where(t => t.DR == accountNo && t.Status == "PENDING").ToList();
                foreach (var trans in pendingTrans)
                {
                    pendingTransAmount += trans.Amount;
                }

                //This checks if the account is funded
                if (Math.Abs(accountDetails.BookBalance) - Math.Abs(amount) - (pendingTransAmount) < 0)
                    accountFunded = false;
                else if (Math.Abs(accountDetails.BookBalance) - Math.Abs(amount) - (pendingTransAmount) >= 0)
                    accountFunded = true;
            }
            else if (transType == "TELLER")
            {
                var accountDetails = _context.ChartOfAccount.Where(c => c.AccountNo == accountNo).FirstOrDefault();

                //This gets the list of pending transactions
                var pendingTrans = _context.Transactions.Where(t => t.DR == accountNo && t.Status == "PENDING").ToList();
                foreach (var trans in pendingTrans)
                {
                    pendingTransAmount += trans.Amount;
                }

                //This checks if the account is funded
                if (Math.Abs(accountDetails.BookBalance) - Math.Abs(amount) - (pendingTransAmount) < 0)
                    accountFunded = false;
                else if (Math.Abs(accountDetails.BookBalance) - Math.Abs(amount) - (pendingTransAmount) >= 0)
                    accountFunded = true;
            }
            return (accountFunded);
        }

        private string GetUserBranchCode(string username)
        {
            return _context.ApplicationUser.Where(u => u.UserName == username)
                .Include(b => b.Profile.Branch)
                .Select(c => c.Profile.Branch.BranchCode)
                .FirstOrDefault();
        }

        public string CheckAccountStatus(string accountNo)
        {
            return _context.Accounts.Where(a => a.ID == accountNo).Select(a => a.Status).FirstOrDefault();
        }

        public bool CheckPostNoDebit(string accountNo)
        {
            return _context.Accounts.Where(a => a.ID == accountNo).Select(a => a.PostNoDebit).FirstOrDefault();
        }

        public bool CheckPostNoCredit(string accountNo)
        {
            return _context.Accounts.Where(a => a.ID == accountNo).Select(a => a.PostNoCredit).FirstOrDefault();
        }

        public bool TransactionLimitReached(string username, decimal amount)
        {
            var transaction = _context.ApplicationUser.Where(u => u.UserName == username).FirstOrDefault();
            if (amount > transaction.TransactionLimit)
                return true;
            else
                return false;
        }

        public string GetTellerAccount(string username)
        {
            return _context.ChartOfAccount.Where(c => c.AccountName == string.Concat("TELLER-", username.ToUpper()).Trim())
                .Select(c => c.AccountNo)
                .FirstOrDefault();
        }

        public bool CheckIfTeller(string username)
        {
            string branchCode = GetUserBranchCode(username);
            var tellerAccount = _context.ChartOfAccount.Where(c => c.AccountName == string.Concat("TELLER ", username.ToUpper()) && c.Branch.BranchCode == branchCode)
                .Include(c => c.Branch)
                .FirstOrDefault();

            if (tellerAccount == null)
                return false;
            else
                return true;
        }

        private long GenerateRefNo()
        {
            var currentTransCounter = _context.Parameter.Where(p => p.Name == "TRANSACTION_COUNTER").FirstOrDefault();

            long returnCounter = Convert.ToInt64(currentTransCounter.Value);

            //This updates the customer number counter
            currentTransCounter.Value = (Convert.ToInt64(currentTransCounter.Value) + 1).ToString();
            _context.SaveChanges();

            return (returnCounter);
        }

        public DateTime GetTransactionDate()
        {
            return Convert.ToDateTime(_context.Parameter.Where(p => p.Name == "TRANSACTION_DATE").Select(p => p.Value).FirstOrDefault());
        }

        private string GetInterBranchAccount(string debitAccount, string creditAccount)
        {
            //This method returns the Inter Branch Account of 'Credit Account' in 'Debit Account' chart of account
            string drBranchCode = GetBranchCodeFromAccountNo(debitAccount);
            string crBranchCode = GetBranchCodeFromAccountNo(creditAccount);

            //This gets the branch description using the branch code
            string branchDesc = _context.Branch.Where(b => b.BranchCode == crBranchCode).Select(b => b.BranchDesc).FirstOrDefault();

            return _context.ChartOfAccount.Where(c => c.Branch.BranchCode == drBranchCode && c.AccountName == string.Concat("INTERBRANCH ", branchDesc)
                     && c.AccountSubHead.AccountName == "INTER BRANCH")
                     .Include(b => b.Branch)
                     .Include(a => a.AccountSubHead)
                     .Select(c => c.AccountNo)
                     .FirstOrDefault();
        }

        private async Task CreateTransactionHistory(string dr, string cr, decimal amount, string narration, Branch fromBranch, Branch toBranch, string instrumentNo, string refNo, string transType, string username)
        {
            Transactions history = new Transactions
            {
                Amount = amount,
                ApprovedBy = "SYSTEM",
                CR = cr,
                CRBal = 0,
                CRBalBF = 0,
                ToBranch = toBranch,
                DR = dr,
                DRBal = 0,
                DRBalBF = 0,
                InstrumentNo = instrumentNo.ToUpper(),
                Narration = narration.ToUpper(),
                FromBranch = fromBranch,
                PostedBy = username.ToUpper(),
                RefNo = refNo,
                Status = "PENDING",
                TransDate = GetTransactionDate(),
                TransType = transType
            };
            _context.Transactions.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfUserHasRole(string username)
        {
            var selectedRole = await _roleManager.FindByNameAsync("CASH TRANSACTION");
            var roleClaims = await _roleManager.GetClaimsAsync(selectedRole);
            var selectedClaim = roleClaims.Where(r => r.Type == "INTER BRANCH").FirstOrDefault();
            //Check if the user already has the claim
            var user = _context.ApplicationUser.Where(a => a.UserName == username).FirstOrDefault();
            var userClaims = await _userManager.GetClaimsAsync(user);
            if (!userClaims.Contains(selectedClaim))
            {
               return false;
            }
            else { return true; }
        }
        #endregion

        //Deposit transactions
        #region
        public bool CheckCashDepositTransactionLimitReached(string accountNo)
        {
            var transactionlimit = _context.Accounts.Where(a => a.ID == accountNo).FirstOrDefault();

            if (transactionlimit.DepositTransactionPeriod == "WEEKLY")
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public bool CheckCashDepositAmountLimitReached(string accountNo)
        {
            var transactionlimit = _context.Accounts.Where(a => a.ID == accountNo).FirstOrDefault();

            if (transactionlimit.DepositTransactionPeriod == "WEEKLY")
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> CreateCashDepositAsync(TransactionViewModel model, string username)
        {
            string instrumentNo = string.Empty;
            string refNo = GenerateRefNo().ToString();
            if (model.InstrumentNo == null)
                instrumentNo = string.Empty;
            else
                instrumentNo = model.InstrumentNo;

            string debitBranchCode = GetBranchCodeFromAccountNo(model.DR);
            string creditBranchCode = GetBranchCodeFromAccountNo(model.CR);
            string usernameBranch = GetUserBranchCode(username);
            Branch fromBranch = new Branch();
            Branch toBranch = new Branch();

            if (debitBranchCode != creditBranchCode)
            {
                if (debitBranchCode != usernameBranch)
                    toBranch = _context.Branch.Where(b => b.BranchCode == debitBranchCode).FirstOrDefault();
                else if (creditBranchCode != username)
                    toBranch = _context.Branch.Where(b => b.BranchCode == creditBranchCode).FirstOrDefault();
            }
            else if (debitBranchCode == creditBranchCode)
                toBranch = fromBranch;

            try
            {
                //Create a transaction log for the customer account
                await CreateTransactionHistory(string.Empty, model.CR, model.Amount, model.Narration, fromBranch, toBranch, instrumentNo, refNo, "DP", username);

                //This creates the liability account entry
                var customerAccount = _context.Accounts.Where(a => a.ID == model.CR).Include(b => b.Branch).FirstOrDefault();

                string liabilityAccountNo = GetLiabilityAccountNo(customerAccount.ProductCode, customerAccount.Branch.BranchCode);
                //Create the GL (Liability and Teller Account) transaction history

                await CreateTransactionHistory(model.DR, liabilityAccountNo, model.Amount, model.Narration, fromBranch, toBranch, instrumentNo, refNo, "DP", username);

                //This determines if transaction involves Inter Branch
                if (fromBranch != toBranch)
                {
                    //This is interbranch transaction. Create a transaction log
                    await CreateTransactionHistory(GetInterBranchAccount(model.CR, model.DR), GetInterBranchAccount(model.DR, model.CR), model.Amount, model.Narration, fromBranch, toBranch, instrumentNo, refNo, "DP", username);
                }

                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public void DropCashTransaction(string refNo)
        {
            var deposit = _context.Transactions.Where(t => t.RefNo == refNo).ToList();
            _context.Transactions.RemoveRange(deposit);
        }

        public IEnumerable GetCashDeposit(string username)
        {
            return _context.Transactions.Where(t => t.PostedBy == username && t.Status == "PENDING" && t.TransType == "DP");
        }

        #endregion

        //Withdrawal transactions
        #region
        public string[] GetAccountMandate(string accountNo)
        {
            string[] accountDetails = new string[2];

            var account = _context.Accounts.Where(a => a.ID == accountNo && a.Status == "ACTIVE").FirstOrDefault();

            accountDetails[0] = string.Concat("/assets/images/signature-mandates/", account.AccountMandate, ".jpg");
            accountDetails[1] = account.StandingOrder;
            return (accountDetails);
        }

        public bool CheckCashWithdrawalTransactionLimitReached(string accountNo)
        {
            var transactionlimit = _context.Accounts.Where(a => a.ID == accountNo).FirstOrDefault();

            if (transactionlimit.DepositTransactionPeriod == "WEEKLY")
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public bool CheckCashWithdrawalAmountLimitReached(string accountNo)
        {
            var transactionlimit = _context.Accounts.Where(a => a.ID == accountNo).FirstOrDefault();

            if (transactionlimit.DepositTransactionPeriod == "WEEKLY")
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> CreateCashWithdrawalAsync(TransactionViewModel model, string username)
        {
            string instrumentNo = string.Empty;
            string refNo = GenerateRefNo().ToString();
            if (model.InstrumentNo == null)
                instrumentNo = string.Empty;
            else
                instrumentNo = model.InstrumentNo;

            string debitBranchCode = GetBranchCodeFromAccountNo(model.DR);
            string creditBranchCode = GetBranchCodeFromAccountNo(model.CR);
            string usernameBranch = GetUserBranchCode(username);
            Branch fromBranch = new Branch();
            Branch toBranch = new Branch();

            if (debitBranchCode != creditBranchCode)
            {
                if (debitBranchCode != usernameBranch)
                    toBranch = _context.Branch.Where(b => b.BranchCode == debitBranchCode).FirstOrDefault();
                else if (creditBranchCode != username)
                    toBranch = _context.Branch.Where(b => b.BranchCode == creditBranchCode).FirstOrDefault();
            }
            else if (debitBranchCode == creditBranchCode)
                toBranch = fromBranch;

            try
            {
                //Create customer transaction history
                await CreateTransactionHistory(model.DR, string.Empty, model.Amount, model.Narration, fromBranch, toBranch, instrumentNo, refNo, "WD", username);

                //This creates the liability account entry
                var customerAccount = _context.Accounts.Where(a => a.ID == model.DR).Include(b => b.Branch).FirstOrDefault();

                string liabilityAccountNo = GetLiabilityAccountNo(customerAccount.ProductCode, customerAccount.Branch.BranchCode);

                //This GL (Liability and Teller) account
                await CreateTransactionHistory(liabilityAccountNo, model.CR, model.Amount, model.Narration, fromBranch, toBranch, instrumentNo, refNo, "WD", username);

                //This determines if transaction involves Inter Branch
                if (fromBranch != toBranch)
                {
                    //This is interbranch transaction. Create a transaction log
                    await CreateTransactionHistory(GetInterBranchAccount(model.CR, model.DR), GetInterBranchAccount(model.DR, model.CR), model.Amount, model.Narration, fromBranch, toBranch, instrumentNo, refNo, "WD", username);
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public IEnumerable GetCashWithdrawal(string username)
        {
            return _context.Transactions.Where(t => t.PostedBy == username && t.Status == "PENDING" && t.TransType == "WD");
        }

        public bool AllowOverdraw(string accountNo)
        {
            return _context.Accounts.Where(a => a.ID == accountNo).Select(a => a.AllowOverdraw).FirstOrDefault();
        }
        #endregion


        //Fund transfer transactions
        #region
        public async Task<string> CreateFundTransferAsync(TransactionViewModel model, string username)
        {
            string instrumentNo = string.Empty;
            string refNo = GenerateRefNo().ToString();
            if (model.InstrumentNo == null)
                instrumentNo = string.Empty;
            else
                instrumentNo = model.InstrumentNo;

            string debitBranchCode = GetBranchCodeFromAccountNo(model.DR);
            string creditBranchCode = GetBranchCodeFromAccountNo(model.CR);
            string usernameBranch = GetUserBranchCode(username);
            Branch fromBranch = new Branch();
            Branch toBranch = new Branch();

            if (debitBranchCode != creditBranchCode)
            {
                if (debitBranchCode != usernameBranch)
                    toBranch = _context.Branch.Where(b => b.BranchCode == debitBranchCode).FirstOrDefault();
                else if (creditBranchCode != username)
                    toBranch = _context.Branch.Where(b => b.BranchCode == creditBranchCode).FirstOrDefault();
            }
            else if (debitBranchCode == creditBranchCode)
                toBranch = fromBranch;

            try
            {
                //Create customer account transaction history
                await CreateTransactionHistory(model.DR, model.CR, model.Amount, model.Narration, fromBranch, toBranch, instrumentNo, refNo, "FT", username);

                //This creates the liability account entry for the debit side
                var drCustomerAccount = _context.Accounts.Where(a => a.ID == model.DR).Include(b => b.Branch).FirstOrDefault(); 

                string drLiabilityAccountNo = GetLiabilityAccountNo(drCustomerAccount.ProductCode, drCustomerAccount.Branch.BranchCode);

                //This creates the liability account entry for the debit side
                var crCustomerAccount = _context.Accounts.Where(a => a.ID == model.CR).Include(b => b.Branch).FirstOrDefault();

                string crLiabilityAccountNo = GetLiabilityAccountNo(crCustomerAccount.ProductCode, crCustomerAccount.Branch.BranchCode);

                //Create the GL (Liability) accounts transaction history
                await CreateTransactionHistory(drLiabilityAccountNo, crLiabilityAccountNo, model.Amount, model.Narration, fromBranch, toBranch, instrumentNo, refNo, "FT", username);

                //This determines if transaction involves Inter Branch
                if (fromBranch != toBranch)
                {
                    //This is interbranch transaction. Create a transaction log
                    await CreateTransactionHistory(GetInterBranchAccount(model.CR, model.DR), GetInterBranchAccount(model.DR, model.CR), model.Amount, model.Narration, fromBranch, toBranch, instrumentNo, refNo, "WD", username);
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public IEnumerable GetFundTransfer(string username)
        {
            return _context.Transactions.Where(t => t.PostedBy == username && t.Status == "PENDING" && t.TransType == "FT");
        }

        #endregion
    }
}
