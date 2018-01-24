using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BankModel.Web;
using BankModel.Web.ViewModels;
using BankModel.Data.Interfaces;

namespace BankModel.Data.Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly DBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        DateTime currentDate;

        public ApprovalRepository(DBContext context, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            currentDate = Convert.ToDateTime(GetTransactionDate());
            _config = config;
        }

        public string GetTransactionDate()
        {
            return _context.Parameter.Where(p => p.Name == "TRANSACTION_DATE").Select(p => p.Value).FirstOrDefault();
        }

        //System users approval details
        #region
        public IEnumerable<SystemUserDetailsViewModel> GetPendingSystemUsers(string username)
        {
            //Get all the pending system users posted by 'PostedBy' with status 'Pending'
            Branch branch = new Branch();
            //Determine if the request is from Super admin who has no profile
            if (username == _config.GetSection("AppInfo")["AdminUsername"])
            {
                branch = _context.Branch.Where(b => b.BranchCode == "00").FirstOrDefault();
            }
            else
            {
                branch = _context.ApplicationUser.Where(u => u.UserName == username)
                .Include(b => b.Profile.Branch)
                .Select(b => b.Profile.Branch)
                .FirstOrDefault();
            }

            var model = (from user in _context.ApplicationUser
                         join profile in _context.Profile on user.Profile.ID equals profile.ID
                                where user.Status == "PENDING" && user.Profile.Branch == branch
                                select new SystemUserDetailsViewModel
                                {
                                    ID = user.Id,
                                    ApprovalLimit = user.ApprovalLimit,
                                    Branch = profile.Branch.BranchDesc,
                                    Email = user.Email,
                                    PasswordExpiryDate = user.PasswordExpiryDate,
                                    PostedBy = user.PostedBy,
                                    Status = user.Status,
                                    TransactionLimit = user.TransactionLimit,
                                    TransDate = user.TransDate,
                                    Username = user.UserName
                                });
            return (model);
        }

        public async Task<string> ApproveSystemUsersAsync(string id, string username)
        {
            try
            {
                var model = await _userManager.FindByIdAsync(id);
                if (model != null)
                {
                    model.Status = "ACTIVE";
                    model.ApprovedBy = username;
                    var result = await _userManager.UpdateAsync(model);
                    if (result.Succeeded)
                        return "Succeeded";
                    else
                        return "Failed";
                }
                else
                    return "Failed";
            }
            catch { return "Failed"; }
        }
        #endregion

        //Branches approval details
        #region
        public IEnumerable<Branch> GetPendingBranches(string username)
        {
            //Get all the pending branches posted by 'PostedBy' with status 'Pending'
            var pendingBranches = (from branch in _context.Branch
                                   where branch.PostedBy == username && branch.Status == "PENDING"
                                   select branch);
            return (pendingBranches);
        }

        public async Task<string> ApproveBranchesAsync(int id, string username)
        {
            try
            {
                var branch = _context.Branch.Where(b => b.ID == id).FirstOrDefault();
                if (branch != null)
                {
                    branch.Status = "ACTIVE";
                    branch.ApprovedBy = username.ToUpper();
                    _context.Branch.Update(branch);
                    await _context.SaveChangesAsync();
                    return "Succeeded";
                }
                else
                    return "Failed";
            }
            catch { return "Failed"; }
        }
        #endregion

        //Chart of account approval details
        #region
        public IEnumerable<BranchChartofAccountViewModel> GetPendingChartofAccount(string username)
        {
            //Get all the pending chart of account in the selected branch with status 'Pending'
            var userBranch = _context.ApplicationUser.Where(u => u.UserName == username)
                .Include(p => p.Profile)
                .Select(u => u.Profile.Branch)
                .FirstOrDefault();

            var model = (from chart in _context.ChartOfAccount
                         join subHead in _context.ChartOfAccountSubHead
                         on chart.AccountSubHead.ID equals subHead.ID
                         where chart.Branch.ID == userBranch.ID && chart.Status == "PENDING"
                         select new BranchChartofAccountViewModel
                         {
                             ID = chart.ID,
                             AccountHead = subHead.AccountHead,
                             AccountName = chart.AccountName,
                             AccountNo = chart.AccountNo,
                             AccountSubHead = subHead.AccountName
                         });
            return (model);
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

        public async Task<string> ApproveChartofAccountAsync(long id, string username)
        {
            try
            {
                var model = _context.ChartOfAccount.Where(c => c.ID == id).Include(c => c.AccountSubHead).Include(c => c.Branch).FirstOrDefault();
                if(model != null)
                {

                    if (model.AccountSubHead.AccountName == "SAVINGS ACCOUNTS")
                    {
                        //Create Interest Expense account along side
                        var accountSubHead = _context.ChartOfAccountSubHead.Where(c => c.AccountHead == "EXPENSE" && c.AccountName == "INTEREST EXPENSE").FirstOrDefault();
                        ChartOfAccount interestExpense = new ChartOfAccount
                        {
                            AccountName = model.AccountName,
                            AccountNo = GenerateChartofAccountNumber(accountSubHead.AccountHead, accountSubHead.AccountName, model.Branch.BranchDesc),
                            AccountSubHead = accountSubHead,
                            ApprovedBy = username.ToUpper(),
                            BookBalance = 0,
                            Branch = model.Branch,
                            PostedBy = username.ToUpper(),
                            Status = "ACTIVE",
                            TransDate = currentDate
                        };
                        _context.ChartOfAccount.Add(interestExpense);
                        await _context.SaveChangesAsync();
                    }

                    if (model.AccountSubHead.AccountName == "CHECKING ACCOUNTS")
                    {
                        //Create Interest Expense account along side
                        var accountSubHead = _context.ChartOfAccountSubHead.Where(c => c.AccountHead == "INCOME" && c.AccountName == "INTEREST INCOME").FirstOrDefault();
                        ChartOfAccount interestIncome = new ChartOfAccount
                        {
                            AccountName = model.AccountName,
                            AccountNo = GenerateChartofAccountNumber(accountSubHead.AccountHead, accountSubHead.AccountName, model.Branch.BranchDesc),
                            AccountSubHead = accountSubHead,
                            ApprovedBy = username.ToUpper(),
                            BookBalance = 0,
                            Branch = model.Branch,
                            PostedBy = username.ToUpper(),
                            Status = "ACTIVE",
                            TransDate = currentDate
                        };
                        _context.ChartOfAccount.Add(interestIncome);
                        await _context.SaveChangesAsync();
                    }

                    if (model.AccountSubHead.AccountName == "LOANS AND ADVANCES")
                    {
                        //Create Loan Interest account along side
                        var accountSubHead = _context.ChartOfAccountSubHead.Where(c => c.AccountHead == "INCOME" && c.AccountName == "INTEREST INCOME").FirstOrDefault();
                        ChartOfAccount interestIncome = new ChartOfAccount
                        {
                            AccountName = model.AccountName,
                            AccountNo = GenerateChartofAccountNumber(accountSubHead.AccountHead, accountSubHead.AccountName, model.Branch.BranchDesc),
                            AccountSubHead = accountSubHead,
                            ApprovedBy = username.ToUpper(),
                            BookBalance = 0,
                            Branch = model.Branch,
                            PostedBy = username.ToUpper(),
                            Status = "ACTIVE",
                            TransDate = currentDate
                        };
                        _context.ChartOfAccount.Add(interestIncome);
                        await _context.SaveChangesAsync();
                    }

                    model.Status = "ACTIVE";
                    model.ApprovedBy = username.ToUpper();
                    _context.ChartOfAccount.Update(model);
                    await _context.SaveChangesAsync();
                    return "Succeeded";
                }
                else
                    return "Failed";
            }
            catch { return "Failed"; }
        }
        #endregion

        //Account Template approval details
        #region
        public IEnumerable<TemplateAccount> GetPendingAccountTemplate(string username)
        {
            //Get all the pending account template 
            var model = (from template in _context.TemplateAccount
                         where template.Status == "PENDING"
                         select new TemplateAccount
                         {
                             ID = template.ID,
                             TemplateName = template.TemplateName,
                             ProductCode = template.ProductCode,
                             PostedBy = template.PostedBy,
                             TransDate = template.TransDate,
                             Status = template.Status
                         });
            return (model);
        }

        public async Task<string> ApproveAccountTemplateAsync(int id, string username)
        {
            try
            {
                var model = _context.TemplateAccount.Where(c => c.ID == id).FirstOrDefault();
                if (model != null)
                {
                    model.Status = "ACTIVE";
                    model.ApprovedBy = username.ToUpper();
                    _context.TemplateAccount.Update(model);
                    await _context.SaveChangesAsync();
                    return "Succeeded";
                }
                else
                    return "Failed";
            }
            catch { return "Failed"; }
        }

        public TemplateAccount GetAccountTemplateDetails(int id)
        {
            return _context.TemplateAccount.Where(t => t.ID == id).FirstOrDefault();
        }
        #endregion

        //Profile approval details
        #region
        public IEnumerable<Profile> GetPendingProfile(string username)
        {
            //Get all the pending account template in the selected branch with status 'Pending'
            var userBranch = _context.ApplicationUser.Where(u => u.UserName == username).Include(p => p.Profile).Select(u => u.Profile.Branch).FirstOrDefault();
            var model = (from profile in _context.Profile
                         where profile.Status == "PENDING"
                         select new Profile
                         {
                             ID = profile.ID,
                             Lastname = profile.Lastname,
                             Othernames = profile.Othernames,
                             Employer = profile.Employer,
                             EmployerAddress = profile.EmployerAddress,
                             Designation = profile.Designation,
                             IncorporationType = profile.IncorporationType,
                             RCNo = profile.RCNo,
                             CustomerType = profile.CustomerType,
                             DateOfBirth = profile.DateOfBirth,
                             PostedBy = profile.PostedBy,
                             TransDate = profile.TransDate
                         });
            return (model);
        }

        public async Task<string> ApproveProfileAsync(string id, string username)
        {
            try
            {
                var model = _context.Profile.Where(c => c.ID == id).FirstOrDefault();
                if (model != null)
                {
                    model.Status = "ACTIVE";
                    model.ApprovedBy = username.ToUpper();
                    _context.Profile.Update(model);
                    await _context.SaveChangesAsync();
                    return "Succeeded";
                }
                else
                    return "Failed";
            }
            catch { return "Failed"; }
        }
        #endregion

        //Customer account approval details
        #region
        public IEnumerable<Account> GetPendingCustomerAccount(string username)
        {
            //Get all the pending account template in the selected branch with status 'Pending'
            var userBranch = _context.ApplicationUser.Where(u => u.UserName == username).Include(p => p.Profile).Select(u => u.Profile.Branch).FirstOrDefault();
            var model = (from account in _context.Accounts
                         where account.Status == "PENDING" && account.Branch == userBranch
                         select new Account
                         {
                             ID = account.ID,
                             AccountType = account.AccountType,
                             AccountOfficer = account.AccountOfficer,
                             ProductCode = account.ProductCode,
                             PostedBy = account.PostedBy,
                             TransDate = account.TransDate,
                             Status = account.Status
                         });
            return (model);
        }

        public async Task<string> ApproveCustomerAccountAsync(string id, string username)
        {
            try
            {
                var model = _context.Accounts.Where(c => c.ID == id).FirstOrDefault();
                if (model != null)
                {
                    model.Status = "ACTIVE";
                    model.ApprovedBy = username.ToUpper();
                    _context.Accounts.Update(model);
                    await _context.SaveChangesAsync();
                    return "Succeeded";
                }
                else
                    return "Failed";
            }
            catch { return "Failed"; }
        }
        #endregion
    }
}
