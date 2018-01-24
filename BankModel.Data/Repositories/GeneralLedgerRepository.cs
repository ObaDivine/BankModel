using BankModel.Data.Interfaces;
using BankModel.Models;
using BankModel.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BankModel.Data.Repositories
{
    public class GeneralLedgerRepository: IGeneralLedgerRepository
    {
        private readonly DBContext _context;

        public GeneralLedgerRepository(DBContext context)
        {
            _context = context;
        }

        public DateTime GetTransactionDate()
        {
            return Convert.ToDateTime(_context.Parameter.Where(p => p.Name == "TRANSACTION_DATE").Select(p => p.Value).FirstOrDefault());
        }

        public IEnumerable<string> GetBranchNamesByUser(string username)
        {
            var userProfile = _context.ApplicationUser
                .Where(p => p.UserName == username)
                .Include(p => p.Profile.Branch)
                .Select(p => p.Profile.Branch)
                .FirstOrDefault();

            //Return all the branches for the super admin account
            if (username == "admin")
                return _context.Branch.Where(b => b.Status == "ACTIVE").Select(b => b.BranchDesc);

            //Return all the branches if the user is a head office staff (Branch code 00)
            if (userProfile.BranchCode == "00")
                return _context.Branch.Where(b => b.Status == "ACTIVE").Select(b => b.BranchDesc);
            else
                return _context.Branch.Where(b => b.Status == "ACTIVE" && b.BranchCode == userProfile.BranchCode).Select(b => b.BranchDesc);
        }

        //Chart of account details
        #region

        public IEnumerable<string> GetAccountSubHeads(string accountHead)
        {
            return _context.ChartOfAccountSubHead.Where(a => a.AccountHead == accountHead).Select(a => a.AccountName);
        }

        public bool IsChartofAccountInUse(long id)
        {
            var chartofAccount = _context.ChartOfAccount.Where(a => a.ID == id).FirstOrDefault();

            //Check if the book balance is not 0
            if (chartofAccount.BookBalance != 0)
                return true;

            //Check if there is any transaction involving the account
            var transactions = _context.Transactions.Where(t => t.DR == chartofAccount.AccountNo || t.CR == chartofAccount.AccountNo).FirstOrDefault();
            return (transactions != null ? true : false);
        }

        public IEnumerable<BranchChartofAccountViewModel> GetChartofAccountByUser(string username)
        {
            //This returns all the chart of account items created by the user
            var branchChartofAccount = (from c in _context.ChartOfAccount
                                        join a in _context.ChartOfAccountSubHead
                                        on c.AccountSubHead.ID equals a.ID
                                        where c.PostedBy == username
                                        select new BranchChartofAccountViewModel
                                        {
                                            AccountHead = a.AccountHead,
                                            AccountName = c.AccountName,
                                            AccountNo = c.AccountNo,
                                            AccountSubHead = a.AccountName,
                                            Status = c.Status,
                                            ID = c.ID
                                        });

            return branchChartofAccount;
        }

        public ChartofAccountViewModel GetChartofAccount(long id)
        {
            var account = _context.ChartOfAccount.Where(a => a.ID == id).FirstOrDefault();
            //var branch = _context.Branch.Where(b => b.ID == account.Branch.ID).FirstOrDefault();
            var chartofAccount = (from acc in _context.ChartOfAccount
                                  join subHead in _context.ChartOfAccountSubHead on acc.AccountSubHead.ID equals subHead.ID
                                  join branch in _context.Branch on acc.Branch.ID equals branch.ID
                                  where acc.ID == id
                                  select new ChartofAccountViewModel
                                  {
                                      AccountHead = subHead.AccountHead,
                                      AccountName = acc.AccountName,
                                      AccountSubHead = subHead.AccountName,
                                      Branch = branch.BranchDesc,
                                      ID = acc.ID
                                  }).FirstOrDefault();

            return chartofAccount;
        }

        public bool ChartofAccountExist(ChartofAccountViewModel model)
        {
            //This checks if the item already exist in the chart of account
            var subHead = _context.ChartOfAccountSubHead
                .Where(a => a.AccountHead == model.AccountHead && a.AccountName == model.AccountSubHead)
                .FirstOrDefault();
            var branch = _context.Branch.Where(b => b.BranchDesc == model.Branch).FirstOrDefault();
            var result = (from chart in _context.ChartOfAccount
                          where chart.AccountSubHead.ID == subHead.ID && chart.AccountName == model.AccountName
                          && chart.Branch.ID == branch.ID  select chart).FirstOrDefault();

            return (result != null ? true : false);
        }

        public string GetBranchCodeFromName(string branchName)
        {
            return _context.Branch.Where(b => b.BranchDesc == branchName).Select(b => b.BranchCode).FirstOrDefault();
        }

        protected int GenerateChartofAccountCounter(string branchCode)
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

        protected string GenerateChartofAccountNumber(ChartofAccountViewModel model)
        {
            //This gets the branch code for the transaction
            string branchCode = GetBranchCodeFromName(model.Branch);

            //This assigns values to the account head (Asset, Liability, Income and Expense)
            string accountHeadCode = string.Empty;
            if (model.AccountHead == "ASSET")
                accountHeadCode = "01";
            else if (model.AccountHead == "LIABILITY")
                accountHeadCode = "02";
            else if (model.AccountHead == "INCOME")
                accountHeadCode = "03";
            else if (model.AccountHead == "EXPENSE")
                accountHeadCode = "04";

            var accountSubHeadCode = _context.ChartOfAccountSubHead.Where(a => a.AccountHead == model.AccountHead && a.AccountName == model.AccountSubHead).Select(a => a.AccountCode).FirstOrDefault();

            string chartofAccountCounter = GenerateChartofAccountCounter(branchCode).ToString();
            if (chartofAccountCounter.Length < 2)
                chartofAccountCounter = string.Concat("0", chartofAccountCounter);

            //This generates the account no in the formant (BranchCode AccountHead AccountSubHead UniqueCounter - 00011119)
            string chartofAccountNo = string.Concat(branchCode, accountHeadCode, accountSubHeadCode, chartofAccountCounter);
            return (chartofAccountNo);
        }

        public async Task<string> CreateChartofAccountAsync(ChartofAccountViewModel model)
        {
            try
            {
                var accountNo = GenerateChartofAccountNumber(model);
                var branch = _context.Branch.Where(b => b.BranchDesc == model.Branch).FirstOrDefault();
                var accountSubHead = _context.ChartOfAccountSubHead.Where(c => c.AccountHead == model.AccountHead && c.AccountName == model.AccountSubHead).FirstOrDefault();
                var newChartItem = new ChartOfAccount
                {
                    AccountNo = accountNo,
                    AccountName = model.AccountName.ToUpper(),
                    BookBalance = 0,
                    AccountSubHead = accountSubHead,
                    Branch = branch,
                    TransDate = GetTransactionDate(),
                    ApprovedBy = string.Empty,
                    PostedBy = model.ActionBy.ToUpper(),
                    Status = "PENDING"
                };

                _context.ChartOfAccount.Add(newChartItem);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> UpdateChartofAccountAsync(ChartofAccountViewModel model)
        {
            try
            {
                var chartofAccount = _context.ChartOfAccount.Where(b => b.ID == model.ID).FirstOrDefault();
                var branch = _context.Branch.Where(b => b.BranchDesc == model.Branch).FirstOrDefault();
                var accountSubHead = _context.ChartOfAccountSubHead.Where(c => c.AccountHead == model.AccountHead && c.AccountName == model.AccountSubHead).FirstOrDefault();
                chartofAccount.AccountName = model.AccountName.ToUpper();
                chartofAccount.AccountSubHead = accountSubHead;
                chartofAccount.Branch = branch;
                chartofAccount.Status = "PENDING";
                chartofAccount.PostedBy = model.ActionBy.ToUpper();
                _context.ChartOfAccount.Update(chartofAccount);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> DropChartofAccountAsync(int id)
        {
            try
            {
                var chartofAccount = _context.ChartOfAccount.Where(c => c.ID == id).Include(s => s.AccountSubHead).Include(b => b.Branch).FirstOrDefault();
                if (chartofAccount != null)
                {

                    _context.ChartOfAccount.Remove(chartofAccount);
                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public bool UsernameExist(string username)
        {
            var result = _context.ApplicationUser.Where(u => u.UserName == username).FirstOrDefault();
            return (result != null ? true : false);
        }

        #endregion
    }
}
