using BankModel.Data.Interfaces;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BankModel.Models;
using BankModel.Models.ViewModels;

namespace BankModel.Data.Repositories
{
    public class SystemAdminRepository : ISystemAdminRepository
    {
        private readonly DBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        public SystemAdminRepository(DBContext context, UserManager<ApplicationUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
        }

        #region
        private string GetCustomerNoFromName(string customerName)
        {
            int firstIndex = customerName.IndexOf("(");
            int secondIndex = customerName.IndexOf(")");
            return (customerName.Substring((firstIndex + 1), ((secondIndex - firstIndex) - 1)));
        }

        public IEnumerable<SystemUserDetailsViewModel> GetSystemUsers()
        {
            var adminUsername = _config.GetSection("AppInfo")["AdminUsername"];
            var users = (from user in _context.ApplicationUser
                         join profile in _context.Profile on user.Profile.ID equals profile.ID
                         where user.UserName != adminUsername
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
            return (users);
        }

        public IEnumerable<string> GetBranchStaff(string branch)
        {
            //This list only branch staff profiles for creating account officer
            return _context.Profile.Where(p => p.Branch.BranchDesc == branch && p.CustomerType == "STAFF")
                .Select(p => string.Concat(p.Lastname, ", ", p.Othernames, " (", p.ID, ")"));
        }

        public async Task<SystemUserDetailsViewModel> GetUserDetails(string username)
        {
            //This method returns details about a system user
            var user = await _userManager.FindByNameAsync(username);
            var userRoles = new List<string>();
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.AddRange(roles);

            var rolePermissions = new List<string>();
            var permissions = await _userManager.GetClaimsAsync(user);
            rolePermissions.AddRange(permissions.Select(c => c.Type));

            var details = (from u in _context.ApplicationUser
                           join p in _context.Profile on u.Profile.Branch.ID equals p.Branch.ID
                           where u.UserName == user.UserName
                           select new SystemUserDetailsViewModel
                           {
                               ApprovalLimit = u.ApprovalLimit,
                               ApprovedBy = u.ApprovedBy,
                               Branch = p.Branch.BranchDesc,
                               Email = u.Email,
                               ID = u.Id,
                               LockoutEnabled = u.LockoutEnabled,
                               PasswordExpiryDate = u.PasswordExpiryDate,
                               PhoneNumber = u.PhoneNumber,
                               PostedBy = u.PostedBy,
                               ProfileImage = p.ProfileImage,
                               Status = u.Status,
                               TransactionLimit = u.TransactionLimit,
                               TransDate = u.TransDate,
                               Username = u.UserName,
                               Roles = userRoles,
                               Permissions = rolePermissions
                           }).FirstOrDefault();
            return (details);
        }

        public async Task<string> CreateSystemUserAsync(SystemUsersViewModel model)
        {
            try
            {
                //Get the customer number from the name
                var customerNo = GetCustomerNoFromName(model.Staff);
                var profile = _context.Profile.Where(p => p.ID == customerNo).Include(b => b.Branch).FirstOrDefault();

                string status = "PENDING";
                string approvedBy = string.Empty;
                if (model.ActivateUser == true)
                {
                    status = "ACTIVE";
                    approvedBy = model.ActionBy.ToUpper();
                }

                ApplicationUser newUser = new ApplicationUser
                {
                    ApprovedBy = approvedBy,
                    Email = profile.Email.ToLower(),
                    UserName = model.Username,
                    PasswordExpiryDate = DateTime.UtcNow.Date.AddDays(GetPasswordValidityDays()),
                    PhoneNumber = profile.PhoneNumber,
                    Profile = profile,
                    TransactionLimit = model.TransactionLimit,
                    ApprovalLimit = model.ApprovalLimit,
                    Status = status,
                    EmailConfirmed = true,
                    PostedBy = model.ActionBy.ToUpper(),
                    TransDate = Convert.ToDateTime(GetTransactionDate())
                };
                var result = await _userManager.CreateAsync(newUser, _config.GetSection("AppInfo")["AdminPassword"]);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, model.Role);
                    //Add claims if user is a Head Office staff
                    if(profile.Branch.BranchCode == "00")
                    {
                        await _userManager.AddClaimAsync(newUser, new Claim("HeadOfficeStaff", "Yes"));
                    }
                }
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Failed"; }            
        }

        public SystemUsersViewModel GetSystemUserWithDetails(string ID)
        {
            return (from user in _context.ApplicationUser
                    where user.Id == ID
                    select new SystemUsersViewModel
                    {
                        ActivateUser = false,
                        ApprovalLimit = user.ApprovalLimit,
                        TransactionLimit = user.TransactionLimit,
                        Username = user.UserName,
                        StatusMessage = string.Empty
                    }).FirstOrDefault();
        }

        public async Task<string> UpdateSystemUserAsync(SystemUsersViewModel model)
        {
            try
            {
                var user = _context.ApplicationUser.Where(u => u.Id == model.ID).FirstOrDefault();
                user.TransactionLimit = model.TransactionLimit;
                user.ApprovalLimit = model.ApprovalLimit;
                user.UserName = model.Username.ToUpper();
                user.TransDate = user.TransDate;
                user.Status = "PENDING";
                user.PostedBy = model.ActionBy.ToUpper();
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();

                return "Succeeded";
            }
            catch { return "Failed"; }
        }

        public async Task<string> DropSystemUserAsync(string id)
        {
            try
            {
                var user = _context.ApplicationUser.Where(u => u.Id == id).FirstOrDefault();
                if (user != null)
                {
                    _context.ApplicationUser.Remove(user);
                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Failed"; }

        }

        public bool IsSystemUserInUse(string id)
        {
            var user = _context.ApplicationUser.Where(u => u.Id == id).FirstOrDefault();

            //Default Admin should not be deleted
            if (user.UserName == _config.GetSection("AppInfo")["AdminUsername"])
                return true;

            //Check possible transactions by the user
            var trans = _context.Transactions.Where(t => t.PostedBy == user.UserName).FirstOrDefault();
            if (trans != null)
                return true;

            //Check if customer account is created by the user
            var account = _context.Accounts.Where(a => a.PostedBy == user.UserName).FirstOrDefault();
            if (account != null)
                return true;

            //Check if the user created any individual or corporate profile
            var profile = _context.Profile.Where(i => i.PostedBy == user.UserName).FirstOrDefault();
            if (profile != null)
                return true;

            //Check if branch has been created by the user
            var branch = _context.Branch.Where(b => b.PostedBy == user.UserName).FirstOrDefault();
            if (branch != null)
                return true;

            //Check fixed deposit
            var fixedDeposit = _context.FixedDeposit.Where(f => f.PostedBy == user.UserName).FirstOrDefault();
            if (fixedDeposit != null)
                return true;

            //Check loans
            var loan = _context.Loan.Where(l => l.PostedBy == user.UserName).FirstOrDefault();
            if (loan != null)
                return true;

            //Check the templates
            var accountTemplate = _context.TemplateAccount.Where(t => t.PostedBy == user.UserName).FirstOrDefault();
            if (accountTemplate != null)
                return true;

            var fixedDepositTemplate = _context.TemplateFixedDeposit.Where(t => t.PostedBy == user.UserName).FirstOrDefault();
            if (fixedDepositTemplate != null)
                return true;

            var loanTemplate = _context.TemplateLoan.Where(t => t.PostedBy == user.UserName).FirstOrDefault();
            if (loanTemplate != null)
                return true;

            var salaryTemplate = _context.TemplateSalary.Where(t => t.PostedBy == user.UserName).FirstOrDefault();
            if (salaryTemplate != null)
                return true;

            return false;
        }

        #endregion

        public int GetPasswordValidityDays()
        {
            return Convert.ToInt16(_context.Parameter.Where(p => p.Name == "PASSWORD_EXPIRY_DAYS").Select(p => p.Value).FirstOrDefault());
        }

        public List<Parameter> GetApplicationParameters()
        {
            return _context.Parameter.ToList();
        }

        public string GetTransactionDate()
        {
            return _context.Parameter.Where(p => p.Name == "TRANSACTION_DATE").Select(p => p.Value).FirstOrDefault();
        }

        public string MaintainSystemUser(SystemUsersViewModel model)
        {
            try
            {
                var user = _context.ApplicationUser.Where(u => u.UserName == model.Username).Include(p => p.Profile).Include(b => b.Profile.Branch).FirstOrDefault();
                if(user != null)
                {
                    if (model.MaintenanceType == "RESET PASSWORD")
                    {
                        _userManager.ResetPasswordAsync(user, "", _config.GetSection("AppInfo")["AdminPassword"]);
                        _context.SaveChanges();
                        //UpdateAuditLog(postedBy.ToUpper(), model.AppUser + " PASSWORD RESET", "SYSTEM ADMIN", "MAINTENANCE", clientIP, "999", DateTime.Now);
                    }

                    if (model.MaintenanceType == "UPDATE TRANSACTION LIMIT")
                    {
                        decimal oldLimit = user.ApprovalLimit;
                        user.TransactionLimit = model.LimitField;
                        _context.SaveChanges();
                        //UpdateAuditLog(postedBy.ToUpper(), model.AppUser + " TRANSACTION LIMIT UPDATED FROM " + oldLimit + " TO " + model.LimitField.ToString("#,##0.00"), "SYSTEM ADMIN", "MAINTENANCE", clientIP, "999", DateTime.Now);
                    }

                    if (model.MaintenanceType == "UPDATE APPROVAL LIMIT")
                    {
                        decimal oldLimit = user.ApprovalLimit;
                        user.ApprovalLimit = model.LimitField;
                        _context.SaveChanges();
                        //UpdateAuditLog(postedBy.ToUpper(), model.AppUser + " APPROVAL LIMIT UPDATED FROM " + oldLimit + " TO " + model.LimitField.ToString("#,##0.00"), "SYSTEM ADMIN", "MAINTENANCE", clientIP, "999", DateTime.Now);
                    }

                    if (model.MaintenanceType == "CLEAR LOGIN")
                    {
                        user.UserOnline = false;
                        _context.SaveChanges();
                        //UpdateAuditLog(postedBy.ToUpper(), model.AppUser + " CLEARED FROM LOGIN", "SYSTEM ADMIN", "MAINTENANCE", clientIP, "999", DateTime.Now);
                    }

                    if (model.MaintenanceType == "MAKE PASSWORD EXPIRE")
                    {
                        user.PasswordExpiryDate = DateTime.Now.Date;
                        _context.SaveChanges();
                        //UpdateAuditLog(postedBy.ToUpper(), model.AppUser + " PASSWORD SET TO EXPIRE ON " + DateTime.Now.Date, "SYSTEM ADMIN", "MAINTENANCE", clientIP, "999", DateTime.Now);
                    }

                    if (model.MaintenanceType == "DISABLE USER")
                    {
                        user.Status = "DISABLED";
                        _context.SaveChanges();
                        //UpdateAuditLog(postedBy.ToUpper(), model.AppUser + " DISABLED", "SYSTEM ADMIN", "MAINTENANCE", clientIP, "999", DateTime.Now);
                    }

                    if (model.MaintenanceType == "ENABLE USER")
                    {
                        user.Status = "ACTIVE";
                        _context.SaveChanges();
                        //UpdateAuditLog(postedBy.ToUpper(), model.AppUser + " ENABLED", "SYSTEM ADMIN", "MAINTENANCE", clientIP, "999", DateTime.Now);
                    }

                    if (model.MaintenanceType == "CHANGE BRANCH")
                    {
                        var branch = _context.Branch.Where(b => b.BranchDesc == model.NewBranch).FirstOrDefault();
                        string oldBranchCode = user.Profile.Branch.BranchCode;
                        user.Profile.Branch = branch;
                        _context.SaveChanges();
                        //UpdateAuditLog(postedBy.ToUpper(), model.AppUser + " BRANCH CHANGED FROM " + oldBranchCode + " TO " + user.BranchCode, "SYSTEM ADMIN", "MAINTENANCE", clientIP, "999", DateTime.Now);
                    }

                    if (model.MaintenanceType == "EXTEND PASSWORD EXPIRY")
                    {
                        user.PasswordExpiryDate = user.PasswordExpiryDate.AddDays(model.PasswordDays);
                        _context.SaveChanges();
                        //UpdateAuditLog(postedBy.ToUpper(), model.AppUser + " PASSWORD EXTENDED BY " + model.PasswordDays + " DAYS", "SYSTEM ADMIN", "MAINTENANCE", clientIP, "999", DateTime.Now);
                    }
                }
                return "Succeeded";
            }
            catch { return "Failed"; }
        }
    }
}
