using BankModel.Data.Interfaces;
using BankModel.Models;
using BankModel.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BankModel.Data.Repositories
{
    public class TemplateRepository: ITemplateRepository
    {
        private readonly DBContext _context;

        public TemplateRepository(DBContext context)
        {
            _context = context;
        }

        public bool ProductCodeExist(string productCode)
        {
            var result  = _context.TemplateAccount.Where(t => t.ProductCode == productCode).FirstOrDefault();
            return (result != null ? true : false);
        }

        public DateTime GetTransactionDate()
        {
            return Convert.ToDateTime(_context.Parameter.Where(p => p.Name == "TRANSACTION_DATE").Select(p => p.Value).FirstOrDefault());
        }

        //Account template processes
        #region

        public IEnumerable<TemplateAccount> GetAccountTemplate(string username)
        {
            //Return all the account template created by the specified user
            return _context.TemplateAccount.Where(t => t.PostedBy == username);
        }

        public TemplateAccountViewModel GetAccountTemplateByID(int ID)
        {
            return (from model in _context.TemplateAccount
                    where model.ID == ID
                    select new TemplateAccountViewModel
                    {
                        ID = model.ID,
                        TemplateName = model.TemplateName,
                        AcceptCheques = model.AcceptCheques,
                        AccountType = model.AccountType,
                        ProductCode = model.ProductCode,
                        ChargeForOverdrawn = model.ChargeForOverdrawn,
                        DepositAmountLimit = model.DepositAmountLimit,
                        DepositAmountPeriod = model.DepositAmountPeriod,
                        DepositTransactionLimit = model.DepositTransactionLimit,
                        DepositTransactionPeriod = model.DepositTransactionPeriod,
                        EmailNotification = model.EmailNotification,
                        InterestDrop = model.InterestDrop,
                        InterestPerAnnum = model.InterestPerAnnum,
                        MinimumBalance = model.MinimumBalance,
                        MonthlyStatement = model.MonthlyStatement,
                        MonthlyStatementBy = model.MonthlyStatementBy,
                        MonthlyStatementCost = model.MonthlyStatementCost,
                        OverdrawnChargeType = model.OverdrawnChargeType,
                        OverdrawnFee = model.OverdrawnFee,
                        PostNoCredit = model.PostNoCredit,
                        PostNoDebit = model.PostNoDebit,
                        SMSCost = model.SMSCost,
                        SMSNotification = model.SMSNotification,
                        UseForFixedDeposit = model.UseForFixedDeposit,
                        UseForLoans = model.UseForLoans,
                        WithdrawalAmountLimit = model.WithdrawalAmountLimit,
                        WithdrawalAmountPeriod = model.WithdrawalAmountPeriod,
                        WithdrawalTransactionLimit = model.WithdrawalTransactionLimit,
                        WithdrawalTransactionPeriod = model.WithdrawalTransactionPeriod,
                        StatusMessage = string.Empty
                    }).FirstOrDefault();
        }

        public bool AccountTemplateExist(string templateName)
        {
            var result = _context.TemplateAccount.Where(t => t.TemplateName == templateName).FirstOrDefault();
            return (result != null ? true : false);
        }

        public bool IsAccountTemplateInUse(int id)
        {
            var template = _context.Accounts.Where(a => a.Template.ID == id).Include(t => t.Template).FirstOrDefault();
            if (template != null)
                return true;

            return false;
        }

        public async Task<string> CreateAccountTemplateAsync(TemplateAccountViewModel model)
        {
            try
            {
                TemplateAccount template = new TemplateAccount
                {
                    TemplateName = model.TemplateName.ToUpper(),
                    AcceptCheques = model.AcceptCheques,
                    AccountType = model.AccountType.ToUpper(),
                    AllowOverdraw = model.AllowOverdraw,
                    ProductCode = model.ProductCode,
                    ChargeForOverdrawn = model.ChargeForOverdrawn,
                    DepositAmountLimit = model.DepositAmountLimit,
                    DepositAmountPeriod = model.DepositAmountPeriod.ToUpper(),
                    DepositTransactionLimit = model.DepositTransactionLimit,
                    DepositTransactionPeriod = model.DepositTransactionPeriod.ToUpper(),
                    EmailNotification = model.EmailNotification,
                    InterestDrop = model.InterestDrop.ToUpper(),
                    InterestPerAnnum = model.InterestPerAnnum,
                    MinimumBalance = model.MinimumBalance,
                    MonthlyStatement = model.MonthlyStatement,
                    MonthlyStatementBy = model.MonthlyStatementBy.ToUpper(),
                    MonthlyStatementCost = model.MonthlyStatementCost,
                    OverdrawnChargeType = model.OverdrawnChargeType.ToUpper(),
                    OverdrawnFee = model.OverdrawnFee,
                    PostNoCredit = model.PostNoCredit,
                    PostNoDebit = model.PostNoDebit,
                    SMSCost = model.SMSCost,
                    SMSNotification = model.SMSNotification,
                    UseForFixedDeposit = model.UseForFixedDeposit,
                    UseForLoans = model.UseForLoans,
                    WithdrawalAmountLimit = model.WithdrawalAmountLimit,
                    WithdrawalAmountPeriod = model.WithdrawalAmountPeriod.ToUpper(),
                    WithdrawalTransactionLimit = model.WithdrawalTransactionLimit,
                    WithdrawalTransactionPeriod = model.WithdrawalTransactionPeriod.ToUpper(),
                    PostedBy = model.ActionBy.ToUpper(),
                    TransDate = GetTransactionDate(),
                    ApprovedBy = string.Empty,
                    Status = "PENDING",      
                };
                _context.TemplateAccount.Add(template);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> UpdateAccountTemplateAsync(TemplateAccountViewModel model)
        {
            try
            {
                var template = _context.TemplateAccount.Where(b => b.ID == model.ID).FirstOrDefault();

                template.TemplateName = model.TemplateName.ToUpper();
                template.AcceptCheques = model.AcceptCheques;
                template.AccountType = model.AccountType.ToUpper();
                template.AllowOverdraw = model.AllowOverdraw;
                template.ProductCode = model.ProductCode;
                template.ChargeForOverdrawn = model.ChargeForOverdrawn;
                template.DepositAmountLimit = model.DepositAmountLimit;
                template.DepositAmountPeriod = model.DepositAmountPeriod.ToUpper();
                template.DepositTransactionLimit = model.DepositTransactionLimit;
                template.DepositTransactionPeriod = model.DepositTransactionPeriod.ToUpper();
                template.EmailNotification = model.EmailNotification;
                template.InterestDrop = model.InterestDrop.ToUpper();
                template.InterestPerAnnum = model.InterestPerAnnum;
                template.MinimumBalance = model.MinimumBalance;
                template.MonthlyStatement = model.MonthlyStatement;
                template.MonthlyStatementBy = model.MonthlyStatementBy.ToUpper();
                template.MonthlyStatementCost = model.MonthlyStatementCost;
                template.OverdrawnChargeType = model.OverdrawnChargeType.ToUpper();
                template.OverdrawnFee = model.OverdrawnFee;
                template.PostNoCredit = model.PostNoCredit;
                template.PostNoDebit = model.PostNoDebit;
                template.SMSCost = model.SMSCost;
                template.SMSNotification = model.SMSNotification;
                template.UseForFixedDeposit = model.UseForFixedDeposit;
                template.UseForLoans = model.UseForLoans;
                template.WithdrawalAmountLimit = model.WithdrawalAmountLimit;
                template.WithdrawalAmountPeriod = model.WithdrawalAmountPeriod.ToUpper();
                template.WithdrawalTransactionLimit = model.WithdrawalTransactionLimit;
                template.WithdrawalTransactionPeriod = model.WithdrawalTransactionPeriod.ToUpper();
                template.PostedBy = model.ActionBy.ToUpper();
                template.TransDate = GetTransactionDate();
                template.ApprovedBy = string.Empty;
                template.Status = "PENDING";
                
                _context.TemplateAccount.Update(template);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> DropAccountTemplateAsync(int id)
        {
            try
            {
                var template = _context.TemplateAccount.Where(b => b.ID == id).FirstOrDefault();
                if (template != null)
                {
                    _context.TemplateAccount.Remove(template);
                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public TemplateAccount GetAccountTemplateDetails(int id)
        {
            return _context.TemplateAccount.Where(t => t.ID == id).FirstOrDefault();
        }
        #endregion
    }
}
