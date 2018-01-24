using BankModel.Data.Interfaces;
using BankModel.Models.ViewModels;
using BankModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BankModel.Data.Repositories
{
    public class SetupRepository : ISetupRepository
    {
        private readonly DBContext _context;
        public SetupRepository(DBContext context)
        {
            _context = context;
        }

        //Branch details
        #region
        public IEnumerable<string> GetBranchNames(string username)
        {
            var userProfile = _context.ApplicationUser
                .Where(p => p.UserName == username)
                .Include(p => p.Profile.Branch)
                .Select(p => p.Profile.Branch)
                .FirstOrDefault();

            //Return all the branches for the super admin account
            if(username == "admin")
                return _context.Branch.Where(b => b.Status == "ACTIVE").Select(b => b.BranchDesc);

            //Return all the branches if the user is a head office staff (Branch code 00)
            if (userProfile.BranchCode == "00")
                return _context.Branch.Where(b => b.Status == "ACTIVE").Select(b => b.BranchDesc);
            else
                return _context.Branch.Where(b => b.Status == "ACTIVE" && b.BranchCode == userProfile.BranchCode).Select(b => b.BranchDesc);
        }

        public IEnumerable<Branch> GetBranchesWithDetails()
        {
            return _context.Branch;
        }

        public BranchViewModel GetBranchWithDetails(int ID)
        {
            return (from branch in _context.Branch
                    where branch.ID == ID
                    select new BranchViewModel
                    {
                        ID = branch.ID,
                        BranchCode = branch.BranchCode,
                        BranchDesc = branch.BranchDesc,
                        BranchLocation = branch.BranchLocation,
                        BranchManager = branch.BranchManager,
                        StatusMessage = string.Empty
                    }).FirstOrDefault();
        }

        public bool CheckIfBranchCodeExist(string branchCode)
        {
            var result =_context.Branch.Where(b => b.BranchCode == branchCode).FirstOrDefault();
            return (result != null ? true : false);
        }

        public bool CheckIfBranchDescriptionExist(string branchDescription)
        {
            var result = _context.Branch.Where(b => b.BranchDesc == branchDescription).FirstOrDefault();
            return (result != null ? true : false);
        }

        public DateTime GetTransactionDate()
        {
            return Convert.ToDateTime(_context.Parameter.Where(p => p.Name == "TRANSACTION_DATE").Select(p => p.Value).FirstOrDefault());
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

        public async Task<string> CreateBranchAsync(BranchViewModel model)
        {
            try
            {
                var newBranch = new Branch
                {
                    BranchCode = model.BranchCode,
                    BranchDesc = model.BranchDesc.ToUpper(),
                    BranchLocation = model.BranchLocation.ToUpper(),
                    BranchManager = model.BranchManager.ToUpper(),
                    ChartofAccountCounter = GenerateChartofAccountCounter(model.BranchCode),
                    PostedBy = model.ActionBy.ToUpper(),
                    Status = "PENDING",
                    TransDate = Convert.ToDateTime(GetTransactionDate()),
                };

                _context.Branch.Add(newBranch);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> UpdateBranchAsync(BranchViewModel model)
        {
            try
            {
                var branch = _context.Branch.Where(b => b.ID == model.ID).FirstOrDefault();
                var branchCode = string.Empty;
                var status = "PENDING";

                //Enforce 00 to be retained for head office. The name can change.
                if (branch.BranchCode == "00")
                {
                    branchCode = "00";
                    status = "ACTIVE";
                }
                else
                    branchCode = model.BranchCode;

                branch.BranchCode = branchCode;
                branch.BranchDesc = model.BranchDesc.ToUpper();
                branch.BranchLocation = model.BranchLocation.ToUpper();
                branch.BranchManager = model.BranchManager.ToUpper();
                branch.TransDate = branch.TransDate;
                branch.Status = status;
                branch.PostedBy = model.ActionBy.ToUpper();
                _context.Branch.Update(branch);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public bool IsBranchInUse(int id)
        {
            var branch = _context.Branch.Where(b => b.ID == id).FirstOrDefault();
            //Check if its the Head office
            if (branch.BranchCode == "00" || branch.BranchDesc == "Head Office")
                return true;

            //Check if there is a chart of account for the branch
            var branchChartofAccount = _context.ChartOfAccount.Where(c => c.Branch == branch).FirstOrDefault();
            if (branchChartofAccount != null)
                return true;

            //Check if there are customers attached to the branch
            var customers = _context.Accounts.Where(a => a.Branch == branch).FirstOrDefault();
            if (customers != null)
                return true;

            return false;
        }

        public async Task<string> DropBranchAsync(int id)
        {
            try
            {
                var branch = _context.Branch.Where(b => b.ID == id).FirstOrDefault();
                if (branch != null)
                {
                    _context.Branch.Remove(branch);
                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        #endregion

        //Account sub head details
        #region
        public bool IsAccountSubHeadInUse(int id)
        {
            var subHead = _context.ChartOfAccountSubHead.Where(s => s.ID == id).FirstOrDefault();
            var account = _context.ChartOfAccount.Where(c => c.AccountSubHead == subHead).FirstOrDefault();
            if (account != null)
                return true;

            return false;
        }

        public IEnumerable<ChartOfAccountSubHead> GetAccountSubHeads()
        {
            return _context.ChartOfAccountSubHead;
        }

        public IEnumerable<string> GetAccountSubHeads(string accountHead)
        {
            return _context.ChartOfAccountSubHead.Where(a => a.AccountHead == accountHead).Select(a => a.AccountName);
        }

        public AccountSubHeadViewModel GetAccountSubHeads(int id)
        {
            return (from c in _context.ChartOfAccountSubHead
                    where c.ID == id
                    select new AccountSubHeadViewModel
                    {
                        AccountCode = c.AccountCode,
                        AccountHead = c.AccountHead,
                        AccountName = c.AccountName,
                        ID = c.ID,
                        ReportingLine = c.ReportingLine

                    }).FirstOrDefault();
        }

        public bool AccountSubHeadCodeExist(string accountCode, string accountHead)
        {
            var result = _context.ChartOfAccountSubHead.Where(b => b.AccountCode == accountCode && b.AccountHead == accountHead).FirstOrDefault();
            return (result != null ? true : false);
        }

        public bool AccountSubHeadNameExist(string accountName, string accountHead)
        {
            var result = _context.ChartOfAccountSubHead.Where(b => b.AccountName == accountName && b.AccountHead == accountHead).FirstOrDefault();
            return (result != null ? true : false);
        }

        public async Task<string> CreateAccountSubHeadAsync(AccountSubHeadViewModel model)
        {
            try
            {
                var newAccount = new ChartOfAccountSubHead
                {
                    AccountCode = model.AccountCode,
                    AccountName = model.AccountName.ToUpper(),
                    AccountHead = model.AccountHead.ToUpper(),
                    ReportingLine = model.ReportingLine.ToUpper()
                };

                _context.ChartOfAccountSubHead.Add(newAccount);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> UpdateAccountSubHeadAsync(AccountSubHeadViewModel model)
        {
            try
            {
                var account = _context.ChartOfAccountSubHead.Where(b => b.ID == model.ID).FirstOrDefault();
                var accountCode = string.Empty;

                account.AccountCode = model.AccountCode;
                account.AccountHead = model.AccountHead.ToUpper();
                account.AccountName = model.AccountName.ToUpper();
                account.ReportingLine = model.ReportingLine.ToUpper();
                _context.ChartOfAccountSubHead.Update(account); 
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> DropAccountSubHeadAsync(int id)
        {
            try
            {
                var account = _context.ChartOfAccountSubHead.Where(b => b.ID == id).FirstOrDefault();
                if (account != null)
                {
                    _context.ChartOfAccountSubHead.Remove(account);
                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Failed"; }

        }

        #endregion


    }
}
