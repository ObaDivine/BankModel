using BankModel.Data.Interfaces;
using BankModel.Models;
using BankModel.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BankModel.Data.Repositories
{
    public class EnquiryRepository: IEnquiryRepository
    {
        private readonly DBContext _context;
        public EnquiryRepository(DBContext context)
        {
            _context = context;
        }

        public EnquiryViewModel GetCustomerAccountDetails(string accountNo)
        {
            return (from a in _context.Accounts
                    join p in _context.Profile on a.Profile equals p
                    join b in _context.Branch on a.Branch equals b
                    where a.ID == accountNo
                    select new EnquiryViewModel
                    {
                        AcceptCheques = a.AcceptCheques,
                        AccountMandate = a.AccountMandate,
                        AccountOfficer = a.AccountOfficer,
                        AccountType = a.AccountType,
                        ApprovedBy = a.ApprovedBy,
                        BookBalance = a.BookBalance,
                        Branch = b.BranchDesc,
                        ChargeForOverdrawn = a.ChargeForOverdrawn,
                        DepositAmountLimit = a.DepositAmountLimit,
                        DepositAmountPeriod = a.DepositAmountPeriod,
                        DepositTransactionLimit = a.DepositTransactionLimit,
                        DepositTransactionPeriod = a.DepositTransactionPeriod,
                        EmailNotification = a.EmailNotification,
                        ID = a.ID,
                        InterestDrop = a.InterestDrop,
                        InterestPerAnnum = a.InterestPerAnnum,
                        MinimumBalance = a.MinimumBalance,
                        MonthlyStatement = a.MonthlyStatement,
                        MonthlyStatementBy = a.MonthlyStatementBy,
                        MonthlyStatementCost = a.MonthlyStatementCost,
                        OverdrawnChargeType = a.OverdrawnChargeType,
                        OverdrawnFee = a.OverdrawnFee,
                        PostedBy = a.PostedBy,
                        PostNoCredit = a.PostNoCredit,
                        PostNoDebit = a.PostNoDebit,
                        ProductCode = a.ProductCode,
                        Profile = p.ID,
                        SMSCost = a.SMSCost,
                        SMSNotification = a.SMSNotification,
                        StandingOrder = a.StandingOrder,
                        Status = a.Status,
                        TransDate = a.TransDate,
                        UseForLoans = a.UseForLoans,
                        UseForFixedDeposit = a.UseForFixedDeposit,
                        WithdrawalAmountLimit = a.WithdrawalAmountLimit,
                        WithdrawalAmountPeriod = a.WithdrawalAmountPeriod,
                        WithdrawalTransactionLimit = a.WithdrawalTransactionLimit,
                        WithdrawalTransactionPeriod = a.WithdrawalTransactionPeriod
                        
                    }).FirstOrDefault();
        }

        public IEnumerable<Account> GetCustomerAccounts(string accountNo)
        {
            var customerNo = _context.Accounts.Where(a => a.ID == accountNo).Include(p => p.Profile).Select(p => p.Profile.ID).FirstOrDefault();
            return _context.Accounts.Where(a => a.Profile.ID == customerNo && a.ID != accountNo && a.Status == "ACTIVE");
        }

        public Profile GetCustomerProfile(string ID)
        {
            return _context.Profile.Where(p => p.ID == ID).FirstOrDefault();
        }

        public IEnumerable GetAccountTransactions(string accountNo)
        {
            var debitTransactions = (from trans in _context.Transactions
                                     where trans.DR == accountNo
                                     select new EnquiryTransactionViewModel
                                     {
                                         Account = trans.DR,
                                         Amount = trans.Amount,
                                         BalanceBF = trans.DRBalBF,
                                         ClosingBalance = trans.DRBal,
                                         ID = trans.ID,
                                         Narration = trans.Narration,
                                         TransDate = trans.TransDate,
                                         TransType = "D"
                                     });

            var creditTransactions = (from trans in _context.Transactions
                                      where trans.CR == accountNo
                                      select new EnquiryTransactionViewModel
                                      {
                                          Account = trans.CR,
                                          Amount = trans.Amount,
                                          BalanceBF = trans.CRBalBF,
                                          ClosingBalance = trans.CRBal,
                                          ID = trans.ID,
                                          Narration = trans.Narration,
                                          TransDate = trans.TransDate,
                                          TransType = "C"
                                      });

            var allTransactions = debitTransactions.Concat(creditTransactions).OrderBy(c => c.ID);

            return (allTransactions);
        }
    }
}
