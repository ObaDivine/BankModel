using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using BankModel.Data;
using BankModel.Web;

namespace BankModel.Data
{
    public class DBInitializer
    {
        private readonly DBContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private IConfiguration _config;
        public DBInitializer(DBContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
        }

        public async Task Seed()
        {
            _context.Database.EnsureCreated();

            if (_context.Branch.Any())
            {
                return; //This means the database has been created already
            }

            //Create the default head office 'Branch'
            var headOffice = new Branch
            {
                ApprovedBy = "Bank.Model",
                BranchCode = "00",
                BranchDesc = _config.GetSection("AppInfo")["DefaultBranchDescription"],
                BranchLocation = _config.GetSection("AppInfo")["DefaultAddress"],
                BranchManager = _config.GetSection("AppInfo")["DefaultBranchManager"],
                PostedBy = "Bank.Model",
                Status = "ACTIVE",
                TransDate = DateTime.UtcNow
            };
            await _context.Branch.AddAsync(headOffice);
            await _context.SaveChangesAsync();

            //Create user roles
            #region
            IdentityResult result;

            //Create Administrator roles and claims
            var adminRoleExist = await _roleManager.RoleExistsAsync("ADMINISTRATOR");
            if (!adminRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("ADMINISTRATOR"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("ADMINISTRATOR");
                    string[] roleClaims = { "SYSTEM USERS", "PARAMETER", "USER ROLE", "REMOTE CONNECTION", "AUDIT LOG" };
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var cashRoleExist = await _roleManager.RoleExistsAsync("CASH TRANSACTION");
            if (!cashRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("CASH TRANSACTION"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("CASH TRANSACTION");
                    string[] roleClaims = { "DEPOSIT", "WITHDRAWAL", "FUND TRANSFER" };
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var customerServiceRoleExist = await _roleManager.RoleExistsAsync("CUSTOMER SERVICE");
            if (!customerServiceRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("CUSTOMER SERVICE"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("CUSTOMER SERVICE");
                    string[] roleClaims = { "CUSTOMER ACCOUNT", "INDIVIDUAL PROFILE", "CORPORATE PROFILE" };
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var routineProcessingRoleExist = await _roleManager.RoleExistsAsync("ROUTINE PROCESSING");
            if (!routineProcessingRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("ROUTINE PROCESSING"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("ROUTINE PROCESSING");
                    string[] roleClaims = { "END OF DAY", "END OF MONTH", "END OF YEAR" };
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var enquiryRoleExist = await _roleManager.RoleExistsAsync("ENQUIRY");
            if (!enquiryRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("ENQUIRY"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("ENQUIRY");
                    string[] roleClaims = { "CUSTOMER ENQUIRY", "GL ENQUIRY" };
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var templateRoleExist = await _roleManager.RoleExistsAsync("TEMPLATE");
            if (!templateRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("TEMPLATE"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("TEMPLATE");
                    string[] roleClaims = { "ACCOUNT TEMPLATE", "LOAN TEMPLATE", "FIXED DEPOSIT TEMPLATE" };
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var reportRoleExist = await _roleManager.RoleExistsAsync("REPORT");
            if (!reportRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("REPORT"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("REPORT");
                    string[] roleClaims = { "TRANSACTION", "MANAGEMENT", "OPERATION", "LOAN", "FIXED DEPOSIT" };
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var approvalRoleExist = await _roleManager.RoleExistsAsync("APPROVAL");
            if (!approvalRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("APPROVAL"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("APPROVAL");
                    string[] roleClaims = { "APPROVE SYSTEM USERS", "APPROVE BRANCH", "APPROVE CHART OF ACCOUNT", "APPROVE PROFILE",
                    "APPROVE ACCOUNT", "APPROVE CASH TRANSACTION", "APPROVE GL TRANSACTION", "APPROVE LOAN",
                    "APPROVE FIXED DEPOSIT", "APPROVE TREASURY", "APPROVE SALARY", "APPROVE MOBILE MONEY", "APPROVE TEMPLATE"};
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var loanRoleExist = await _roleManager.RoleExistsAsync("LOAN");
            if (!loanRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("LOAN"));
            }

            var treasuryRoleExist = await _roleManager.RoleExistsAsync("TREASURY");
            if (!treasuryRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("TREASURY"));
            }

            var fixedDepositRoleExist = await _roleManager.RoleExistsAsync("FIXED DEPOSIT");
            if (!fixedDepositRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("FIXED DEPOSIT"));
            }

            var glTransRoleExist = await _roleManager.RoleExistsAsync("GL TRANSACTION");
            if (!glTransRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("GL TRANSACTION"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("GL TRANSACTION");
                    string[] roleClaims = { "CHART OF ACCOUNT" };
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var salaryRoleExist = await _roleManager.RoleExistsAsync("SALARY");
            if (!salaryRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("SALARY"));
            }

            var setupRoleExist = await _roleManager.RoleExistsAsync("SETUP");
            if (!setupRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("SETUP"));
                if (result.Succeeded)
                {
                    var newRole = await _roleManager.FindByNameAsync("SETUP");
                    string[] roleClaims = { "ACCOUNT SUBHEAD", "BRANCH" };
                    foreach (var claim in roleClaims)
                    {
                        await _roleManager.AddClaimAsync(newRole, new Claim(claim, claim, "BankModel", issuer: "BankModel"));
                    }
                }
            }

            var productRoleExist = await _roleManager.RoleExistsAsync("PRODUCT");
            if (!productRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("PRODUCT"));
            }

            var mobileMoneyRoleExist = await _roleManager.RoleExistsAsync("MOBILE MONEY");
            if (!mobileMoneyRoleExist)
            {
                result = await _roleManager.CreateAsync(new IdentityRole("MOBILE MONEY"));
            }

            #endregion

            //This seed the application parameters
            #region

            //This seed the database
            var parameters = new Parameter[]
                {
                new Parameter{Name = "TRANSACTION_DATE", Value = DateTime.Now.Date.ToString("d"), Description = "Current business date"},
                new Parameter{Name = "TRANSACTION_COUNTER", Value = "0", Description = "Incremental counter for RefNo"},
                new Parameter{Name = "CUSTOMER_COUNTER", Value = "0", Description = "Incremental counter for Customer number"},
                new Parameter{Name = "INDIVIDUAL_CUSTOMER_PREFIX", Value = "IND", Description = "Prefix for individual customer number"},
                new Parameter{Name = "CORPORATE_CUSTOMER_PREFIX", Value = "COR", Description = "Prefix for corporate customer number"},
                new Parameter{Name = "COOPERATIVE_CUSTOMER_PREFIX", Value = "COP", Description = "Prefix for cooperative customer number"},
                new Parameter{Name = "PASSWORD_EXPIRY_DAYS", Value = "30", Description = "Password expiry in days"},
                new Parameter{Name = "ACCOUNT_DORMANT_DAYS", Value = "90", Description = "Days before account is flagged dormant"},
                new Parameter{Name = "RETAINED_EARNINGS_PERCENTAGE", Value = "30", Description = "Percentage for calculating retain earnings"},
                new Parameter{Name = "BACKGROUND_COLOR", Value = "white", Description = "Background color customization"},
                new Parameter{Name = "SMS_GATEWAY", Value = "http://www.sms.bbnplace.com/bulksms/bulksms.php?", Description = "Gateway for SMS alert"},
                new Parameter{Name = "SMS_USERNAME", Value = "admin@BankModel.com", Description = "SMS gateway username"},
                new Parameter{Name = "SMS_PASSWORD", Value = "BankModel", Description = "SMS gateway password"},
                new Parameter{Name = "SMS_SENDER_ID", Value = "BankModel", Description = "SMS alert ID"},
                new Parameter{Name = "ROUTINE_PROCESSING_ON", Value = "True", Description = "Indicates if EOD/EOM is running"}
                };
                await _context.Parameter.AddRangeAsync(parameters);
                await _context.SaveChangesAsync();

                #endregion

            //Seed chart of account items
            #region

                var subHeads = new ChartOfAccountSubHead[]
                {
                new ChartOfAccountSubHead{AccountCode = "01", AccountHead = "ASSET", AccountName = "CASH BALANCES", ReportingLine = "CURRENT ASSETS"},
                new ChartOfAccountSubHead{AccountCode = "02", AccountHead = "ASSET", AccountName = "BANK BALANCES", ReportingLine = "CURRENT ASSETS"},
                new ChartOfAccountSubHead{AccountCode = "03", AccountHead = "ASSET", AccountName = "LOANS AND ADVANCES", ReportingLine = "CURRENT ASSET"},
                new ChartOfAccountSubHead{AccountCode = "04", AccountHead = "ASSET", AccountName = "FIXED ASSETS", ReportingLine = "FIXED ASSETS"},
                new ChartOfAccountSubHead{AccountCode = "05", AccountHead = "ASSET", AccountName = "PLACEMENTS", ReportingLine = "CURRENT ASSETS"},
                new ChartOfAccountSubHead{AccountCode = "06", AccountHead = "ASSET", AccountName = "TREASURY AND INVESTMENTS", ReportingLine = "LONG-TERM INVESTMENTS"},
                new ChartOfAccountSubHead{AccountCode = "07", AccountHead = "ASSET", AccountName = "LEASES", ReportingLine = "CURRENT ASSETS"},
                new ChartOfAccountSubHead{AccountCode = "08", AccountHead = "ASSET", AccountName = "INTER BRANCH", ReportingLine = "CURRENT ASSETS"},
                new ChartOfAccountSubHead{AccountCode = "09", AccountHead = "ASSET", AccountName = "OTHER ASSETS", ReportingLine = "OTHER ASSETS"},
                new ChartOfAccountSubHead{AccountCode = "01", AccountHead = "LIABILITY", AccountName = "SAVINGS ACCOUNTS", ReportingLine = "CURRENT LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "02", AccountHead = "LIABILITY", AccountName = "CHECKING ACCOUNTS", ReportingLine = "CURRENT LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "03", AccountHead = "LIABILITY", AccountName = "ACCUMULATED DEPRECIATIONS", ReportingLine = "CURRENT LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "04", AccountHead = "LIABILITY", AccountName = "TAXES AND ACCRUALS", ReportingLine = "CURRENT LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "05", AccountHead = "LIABILITY", AccountName = "PROVISION", ReportingLine = "CURRENT LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "06", AccountHead = "LIABILITY", AccountName = "FIXED DEPOSITS", ReportingLine = "CURRENT LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "08", AccountHead = "LIABILITY", AccountName = "INTEREST PAYABLE", ReportingLine = "CURRENT LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "09", AccountHead = "LIABILITY", AccountName = "GUARANTEES", ReportingLine = "LONG-TERM LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "10", AccountHead = "LIABILITY", AccountName = "CONTINGENT LIABILITY", ReportingLine = "LONG-TERM LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "11", AccountHead = "LIABILITY", AccountName = "LONG TERM LIABILITY", ReportingLine = "LONG-TERM LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "12", AccountHead = "LIABILITY", AccountName = "SHAREHOLDERS FUND", ReportingLine = "SHAREHOLDERS EQUITY"},
                new ChartOfAccountSubHead{AccountCode = "13", AccountHead = "LIABILITY", AccountName = "SHARE PREMIUM", ReportingLine = "SHAREHOLDERS EQUITY"},
                new ChartOfAccountSubHead{AccountCode = "14", AccountHead = "LIABILITY", AccountName = "RETAINED EARNINGS", ReportingLine = "SHAREHOLDERS EQUITY"},
                new ChartOfAccountSubHead{AccountCode = "15", AccountHead = "LIABILITY", AccountName = "PROFIT AND LOSS", ReportingLine = "SHAREHOLDERS EQUITY"},
                new ChartOfAccountSubHead{AccountCode = "16", AccountHead = "LIABILITY", AccountName = "OTHER LIABILITY", ReportingLine = "CURRENT LIABILITY"},
                new ChartOfAccountSubHead{AccountCode = "01", AccountHead = "INCOME", AccountName = "INTEREST INCOME", ReportingLine = "PROFIT AND LOSS"},
                new ChartOfAccountSubHead{AccountCode = "02", AccountHead = "INCOME", AccountName = "FEES AND COMMISSIONS", ReportingLine = "PROFIT AND LOSS"},
                new ChartOfAccountSubHead{AccountCode = "03", AccountHead = "INCOME", AccountName = "OTHER INCOME", ReportingLine = "PROFIT AND LOSS"},
                new ChartOfAccountSubHead{AccountCode = "01", AccountHead = "EXPENSE", AccountName = "BANK AND FINANCIAL CHARGES", ReportingLine = "PROFIT AND LOSS"},
                new ChartOfAccountSubHead{AccountCode = "02", AccountHead = "EXPENSE", AccountName = "INTEREST EXPENSE", ReportingLine = "PROFIT AND LOSS"},
                new ChartOfAccountSubHead{AccountCode = "03", AccountHead = "EXPENSE", AccountName = "PAYROLL EXPENSES", ReportingLine = "PROFIT AND LOSS"},
                new ChartOfAccountSubHead{AccountCode = "04", AccountHead = "EXPENSE", AccountName = "ADMINISTRATIVE EXPENSES", ReportingLine = "PROFIT AND LOSS"},
                new ChartOfAccountSubHead{AccountCode = "05", AccountHead = "EXPENSE", AccountName = "DEPRECIATION COST", ReportingLine = "PROFIT AND LOSS"},
                new ChartOfAccountSubHead{AccountCode = "06", AccountHead = "EXPENSE", AccountName = "OTHER EXPENSES", ReportingLine = "PROFIT AND LOSS"}
                };
                await _context.ChartOfAccountSubHead.AddRangeAsync(subHeads);
                await _context.SaveChangesAsync();

                #endregion

            //Seed States in Nigeria and LGA
            #region
                var states = new State[]
                {
                new State{States = "ABIA", LGAs = "UMUAHIA"},
                };
                await _context.State.AddRangeAsync(states);
                await _context.SaveChangesAsync();
                #endregion

            //Create the super admin
            #region
                var user = await _userManager.FindByEmailAsync(_config.GetSection("AppInfo")["AdminEmail"]);
                if (user == null)
                {
                    var superAdmin = new ApplicationUser
                    {
                        Email = _config.GetSection("AppInfo")["AdminEmail"],
                        EmailConfirmed = true,
                        PasswordExpiryDate = DateTime.UtcNow.AddDays(30),
                        Status = "ACTIVE",
                        UserName = _config.GetSection("AppInfo")["AdminUsername"],
                        TransDate = DateTime.UtcNow.Date,
                        PostedBy = "SYSTEM"
                    };
                    var userResult = await _userManager.CreateAsync(superAdmin, _config.GetSection("AppInfo")["AdminPassword"]);
                    if (userResult.Succeeded)
                    {
                        //Add the admin role to the user by default
                        await _userManager.AddToRoleAsync(superAdmin, "ADMINISTRATOR");
                        await _userManager.AddToRoleAsync(superAdmin, "APPROVAL");

                        //Get the claim for approving new System Users the admin will create
                        var superAdminRole = await _roleManager.FindByNameAsync("ADMINISTRATOR");
                        var superAdminClaims = await _roleManager.GetClaimsAsync(superAdminRole);

                        //Add claim that the Administrator is a Head Office staff
                        await _userManager.AddClaimAsync(superAdmin, new Claim("HeadOfficeStaff", "Yes"));
                        await _userManager.AddClaimAsync(superAdmin, superAdminClaims.Where(c => c.Type == "SYSTEM USERS").FirstOrDefault());
                        await _userManager.AddClaimAsync(superAdmin, superAdminClaims.Where(c => c.Type == "USER ROLE").FirstOrDefault());

                    }
                }
                await _context.SaveChangesAsync();
                #endregion
            }

        }
    }

