using BankModel.Data.Interfaces;
using BankModel.Models;
using BankModel.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankModel.Data.Repositories
{
    public class CustomerServiceRepository: ICustomerServiceRepository
    {
        private readonly DBContext _context;
        private string generatedCustomerNo;
        private string generatedAccountNo;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerServiceRepository(DBContext context, IConfiguration config, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _config = config;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IEnumerable<string> GetStates()
        {
            return _context.State.Select(s => s.States).Distinct();
        }

        public IEnumerable<string> GetLGAs(string state)
        {
            return _context.State.Where(s => s.States == state).Select(l => l.LGAs).Distinct();
        }

        private string GetCustomerNoFromName(string customerName)
        {
            int firstIndex = customerName.IndexOf("(");
            int secondIndex = customerName.IndexOf(")");
            return (customerName.Substring((firstIndex + 1), ((secondIndex - firstIndex) - 1)));
        }

        public bool IsProfileInUse(string id)
        {
            var customerNo = _context.Accounts.Where(a => a.Profile.ID == id).Include(p => p.Profile).FirstOrDefault();
            return (customerNo != null ? true : false);
        }

        public DateTime GetTransactionDate()
        {
            return Convert.ToDateTime(_context.Parameter.Where(p => p.Name == "TRANSACTION_DATE").Select(p => p.Value).FirstOrDefault());
        }

        public string GenerateCustomerNo(string customerType)
        {
            var currentCustomerCounter = _context.Parameter.Where(p => p.Name == "CUSTOMER_COUNTER").FirstOrDefault();

            var customerNoPrefix = _context.Parameter.Where(p => p.Name == customerType).Select(p => p.Value).FirstOrDefault();

            string customerNo = string.Concat(customerNoPrefix, "-", currentCustomerCounter.Value);

            //This updates the customer number counter
            currentCustomerCounter.Value = (Convert.ToInt64(currentCustomerCounter.Value) + 1).ToString();
            _context.SaveChanges();

            return (customerNo);
        }

        public string GetGeneratedCustomerNo()
        {
            return (generatedCustomerNo);
        }

        public Profile GetProfileDetails(string id)
        {
            return _context.Profile.Where(p => p.ID == id).Include(b => b.Branch).FirstOrDefault();
        }

        //Handle customer account
        #region
        public Account GetAccountDetails(string id)
        {
            return _context.Accounts.Where(a => a.ID == id).Include(b => b.Branch).Include(t => t.Template).Include(p => p.Profile).FirstOrDefault();
        }

        public IEnumerable<string> GetCustomers(string customerType, string username)
        {
            var branch = _context.ApplicationUser.Where(u => u.UserName == username).Include(b => b.Profile).Select(b => b.Profile.Branch).FirstOrDefault();
            if (customerType == "INDIVIDUAL")
                return _context.Profile.Where(p => p.Branch == branch && p.CustomerType == "INDIVIDUAL" && p.Status == "ACTIVE").Select(b => string.Concat(b.Lastname, ", ", b.Othernames, " (", b.ID, ")"));
            else if(customerType == "STAFF")
                return _context.Profile.Where(p => p.Branch == branch && p.CustomerType == "STAFF" && p.Status == "ACTIVE").Select(b => string.Concat(b.Lastname, ", ", b.Othernames, " (", b.ID, ")"));
            else if (customerType == "CORPORATE")
                return _context.Profile.Where(p => p.Branch == branch && p.CustomerType == "CORPORATE" && p.Status == "ACTIVE").Select(b => string.Concat(b.Lastname, " (", b.ID, ")"));
            else
                return _context.Profile.Where(p => p.Branch == branch && p.CustomerType == "BankModel" && p.Status == "ACTIVE").Select(b => string.Concat(b.Lastname, ", ", b.Othernames, " (", b.ID, ")"));
        }

        public IEnumerable<string> GetAccountTemplateNames()
        {
            return _context.TemplateAccount.Where(t => t.Status == "ACTIVE").Select(t => t.TemplateName);
        }

        public IEnumerable<string> GetBranchStaff(string username)
        {
            //This list only branch staff profiles for creating account officer
            var branch = _context.ApplicationUser.Where(u => u.UserName == username)
                .Include(b => b.Profile)
                .Select(b => b.Profile.Branch)
                .FirstOrDefault();

            return _context.Profile.Where(p => p.CustomerType == "STAFF" && p.Status == "ACTIVE" && p.Branch.ID == branch.ID)
                .Select(b => string.Concat(b.Lastname, ", ", b.Othernames, " (", b.ID, ")"));
        }

        public string GetGeneratedAccountNo()
        {
            return generatedAccountNo;
        }

        public Account CheckIfCustomerAccountExist(string customerName, string product)
        {
            string customerNo = GetCustomerNoFromName(customerName);
            string productCode = _context.TemplateAccount.Where(t => t.TemplateName == product).Select(t => t.ProductCode).FirstOrDefault();
            return _context.Accounts.Where(a => a.Profile.ID == customerNo && a.ProductCode == productCode).Include(p => p.Profile).Include(t => t.Template).FirstOrDefault();
        }

        public bool IsAccountInUse(string id)
        {
            var account = _context.Accounts.Where(a => a.ID == id && a.Status == "ACTIVE").FirstOrDefault();
            if (account != null)
                return true;
            else
                return false;
        }

        public async Task<bool> IsAdministrator(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            //Get all the roles of the user
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles != null)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var headOfficeClaim = userClaims.Where(c => c.Type == "HeadOfficeStaff" && c.Value == "Yes").FirstOrDefault();
                var adminRole = await _roleManager.FindByNameAsync("ADMINISTRATOR");
                //Check if the user has administrator role
                if (userRoles.Contains(adminRole.Name) && headOfficeClaim != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private string GetAccountOfficerFromName(string customerName)
        {
            return (customerName.Substring(0, (customerName.IndexOf("(") - 1))).Trim();
        }

        public string GenerateAccountNo(string customerName, string product, string username)
        {
            string accountNo = string.Empty;
            string branchCode = _context.ApplicationUser.Where(a => a.UserName == username).Include(b => b.Profile).Select(b => b.Profile.Branch.BranchCode).FirstOrDefault();
            string customerNo = GetCustomerNoFromName(customerName);

            string customerIndexNo = customerNo.Substring((customerNo.IndexOf("-") + 1), (customerNo.Length - (customerNo.IndexOf("-") + 1)));

            if (customerIndexNo.Length == 1)
                customerNo = string.Concat("000", customerIndexNo);
            else if (customerIndexNo.Length == 2)
                customerNo = string.Concat("00", customerIndexNo);
            else if (customerIndexNo.Length == 3)
                customerNo = string.Concat("0", customerIndexNo);

            string productCode = _context.TemplateAccount.Where(t => t.TemplateName == product).Select(t => t.ProductCode).FirstOrDefault();

            //This generates a random 2 digit number
            Random rnd = new Random(1);
            int customerIndex = rnd.Next(10, 100);

            accountNo = string.Concat(branchCode, productCode, customerNo, customerIndex);

            var accountNumber = _context.Accounts.Where(a => a.ID == accountNo).Select(a => a.ID).FirstOrDefault();

            //This checks if the generated account number exist already. If it exist and new random number is regenerated
            if (accountNumber != null)
            {
                Random newRnd = new Random(2);
                int newCustomerIndex = newRnd.Next(10, 100);
                accountNo = string.Concat(branchCode, productCode, customerNo, newCustomerIndex);
            }

            return (accountNo);
        }

        public async Task<string> CreateCustomerAccountAsync(CustomerAccountViewModel model)
        {
            try
            {
                string accountNo = GenerateAccountNo(model.Customer, model.Product, model.ActionBy);
                generatedAccountNo = accountNo;

                string standingOrder = string.Empty;
                //This checks if standing order is null
                if (model.StandingOrder == null)
                    standingOrder = string.Empty;
                else
                    standingOrder = model.StandingOrder;

                var accountTemplate = _context.TemplateAccount.Where(t => t.TemplateName == model.Product).FirstOrDefault();
                var branch = _context.ApplicationUser.Where(u => u.UserName == model.ActionBy).Include(b => b.Profile).Select(b => b.Profile.Branch).FirstOrDefault();
                var customerNo = GetCustomerNoFromName(model.Customer);
                var profile = _context.Profile.Where(p => p.ID == customerNo).FirstOrDefault();
                Account account  = new Account
                {
                    ID = accountNo,
                    AcceptCheques = accountTemplate.AcceptCheques,
                    AccountMandate = accountNo,
                    AccountOfficer = GetAccountOfficerFromName(model.AccountOfficer.ToUpper()),
                    AccountType = accountTemplate.AccountType.ToUpper(),
                    ApprovedBy = string.Empty,
                    BookBalance = 0,
                    Branch = branch,
                    ChargeForOverdrawn = accountTemplate.ChargeForOverdrawn,
                    Profile = profile,
                    DepositAmountLimit = accountTemplate.DepositAmountLimit,
                    DepositAmountPeriod = accountTemplate.DepositAmountPeriod,
                    DepositTransactionLimit = accountTemplate.DepositTransactionLimit,
                    DepositTransactionPeriod = accountTemplate.DepositTransactionPeriod,
                    EmailNotification = accountTemplate.EmailNotification,
                    InterestDrop = accountTemplate.InterestDrop,
                    InterestPerAnnum = accountTemplate.InterestPerAnnum,
                    MinimumBalance = accountTemplate.MinimumBalance,
                    MonthlyStatement = accountTemplate.MonthlyStatement,
                    MonthlyStatementBy = accountTemplate.MonthlyStatementBy,
                    MonthlyStatementCost = accountTemplate.MonthlyStatementCost,
                    OverdrawnChargeType = accountTemplate.OverdrawnChargeType,
                    OverdrawnFee = accountTemplate.OverdrawnFee,
                    PostNoCredit = accountTemplate.PostNoCredit,
                    PostNoDebit = accountTemplate.PostNoDebit,
                    ProductCode = accountTemplate.ProductCode,
                    SMSCost = accountTemplate.SMSCost,
                    SMSNotification = accountTemplate.SMSNotification,
                    StandingOrder = standingOrder.ToUpper(),
                    UseForFixedDeposit = accountTemplate.UseForFixedDeposit,
                    UseForLoans = accountTemplate.UseForLoans,
                    WithdrawalAmountLimit = accountTemplate.WithdrawalAmountLimit,
                    WithdrawalAmountPeriod = accountTemplate.WithdrawalAmountPeriod,
                    WithdrawalTransactionLimit = accountTemplate.WithdrawalTransactionLimit,
                    WithdrawalTransactionPeriod = accountTemplate.WithdrawalTransactionPeriod,
                    PostedBy = model.ActionBy.ToUpper(),
                    TransDate = GetTransactionDate(),
                    Template = accountTemplate,
                    Status = "PENDING"
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Falied"; }
        }

        public async Task<string> UpdateCustomerAccountAsync(CustomerAccountViewModel model)
        {
            try
            {
                var account = _context.Accounts.Where(b => b.ID == model.ID).FirstOrDefault();

                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Falied"; }
        }

        public async Task<string> DropCustomerAccountAsync(string id)
        {
            try
            {
                var account = _context.Accounts.Where(b => b.ID == id).FirstOrDefault();
                if (account != null)
                {
                    _context.Accounts.Remove(account);
                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Falied"; }
        }

        public IEnumerable<Account> GetCustomerAccount(string username)
        {
            //Return all the accounts created by the specified user
            return _context.Accounts.Where(p => p.PostedBy == username).Include(p => p.Profile).Include(b => b.Branch);
        }

        public CustomerAccountViewModel GetCustomerAccount(string id, int customer = 0)
        {
            //Return all the individual profile by ID
            return (from account in _context.Accounts
                    where account.ID == id
                    select new CustomerAccountViewModel
                    {
                        ID = account.ID,
                        AccountOfficer = account.AccountOfficer,
                        StandingOrder = account.StandingOrder,
                        StatusMessage = string.Empty
                    }).FirstOrDefault();

        }

        public IEnumerable<string> GetSystemUsers(string username)
        {
            var branch = _context.ApplicationUser.Where(u => u.UserName == username).Include(b => b.Profile).Select(b => b.Profile.Branch).FirstOrDefault();
            return _context.ApplicationUser.Where(b => b.Profile.Branch == branch).Select(u => u.UserName);
        }

        public IEnumerable<string> GetBranchNames()
        {
            return _context.Branch.Where(b => b.Status == "ACTIVE").Select(b => b.BranchDesc);
        }

        #endregion

        //Handle individual profile
        #region
        public bool CheckIfProfileExist(string lastname, string othername, DateTime dateofBirth)
        {
            var result = _context.Profile.Where(p => p.Lastname == lastname && p.Othernames == othername && p.DateOfBirth == dateofBirth).FirstOrDefault();
            return (result != null ? true : false);
        }

        public bool CheckIfPhoneNumberExist(string phoneNumber)
        {
            var result = _context.Profile.Where(p => p.PhoneNumber == phoneNumber).FirstOrDefault();
            return (result != null ? true : false);
        }

        public bool CheckIfEmailExist(string email)
        {
            var result = _context.Profile.Where(p => p.Email == email).FirstOrDefault();
            return (result != null ? true : false);
        }

        public async Task<string> CreateIndividualProfileAsync(IndividualProfileViewModel model)
        {
            try
            {
                //This generates customer number for the user
                string customerNo = GenerateCustomerNo("INDIVIDUAL_CUSTOMER_PREFIX");
                generatedCustomerNo = customerNo;
                string email = string.Empty;

                if (model.Email == null)
                    email = string.Empty;
                else
                    email = model.Email;

                Branch branch = new Branch();
                //Determine if the request is from Super admin who has no profile
                if(model.Branch == null)
                {
                    if (model.ActionBy == _config.GetSection("AppInfo")["AdminUsername"])
                    {
                        branch = _context.Branch.Where(b => b.BranchCode == "00").FirstOrDefault();
                    }

                    branch = _context.ApplicationUser.Where(u => u.UserName == model.ActionBy)
                        .Include(b => b.Profile.Branch)
                        .Select(b => b.Profile.Branch)
                        .FirstOrDefault();
                }
                else
                {
                    branch = _context.Branch.Where(b => b.BranchDesc == model.Branch).FirstOrDefault();
                }
                
                Profile profile = new Profile
                {
                    ID = customerNo,
                    Lastname = model.Lastname.ToUpper(),
                    Othernames = model.Othernames.ToUpper(),
                    ContactAddress = model.ContactAddress.ToUpper(),
                    CustomerType = model.CustomerType.ToUpper(),
                    Branch = branch,
                    DateOfBirth = model.DateOfBirth,
                    Designation = model.Designation.ToUpper(),
                    Employer = model.Employer.ToUpper(),
                    Email = email.ToUpper(),
                    EmployerAddress = model.EmployerAddress.ToUpper(),
                    Gender = model.Gender,
                    Nationality = model.Nationality,
                    StateOfOrigin = model.StateOfOrigin,
                    LGA = model.LGA,
                    HomeTown = model.HomeTown.ToUpper(),
                    MaritalStatus = model.MaritalStatus,
                    NextOfKin = model.NextofKin.ToUpper(),
                    NokAddress = model.NoKAddress.ToUpper(),
                    NoKPhoneNumber = model.NoKPhoneNumber,
                    NokRelationship = model.NoKRelationship.ToUpper(),
                    PhoneNumber = model.PhoneNumber,
                    ProfileImage = customerNo + ".jpg",
                    Sector = model.Sector,
                    Title = model.Title,
                    PostedBy = model.ActionBy.ToUpper(),
                    TransDate = GetTransactionDate(),
                    ApprovedBy = string.Empty,
                    Status = "PENDING",
                };

                _context.Profile.Add(profile);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Falied"; }
        }

        public async Task<string> UpdateIndividualProfileAsync(IndividualProfileViewModel model)
        {
            try
            {
                string email = string.Empty;

                if (model.Email == null)
                    email = string.Empty;
                else
                    email = model.Email;

                var profile = _context.Profile.Where(b => b.ID == model.ID).FirstOrDefault();
                profile.Lastname = model.Lastname.ToUpper();
                profile.Othernames = model.Othernames.ToUpper();
                profile.CustomerType = model.CustomerType.ToUpper();
                profile.ContactAddress = model.ContactAddress.ToUpper();
                profile.DateOfBirth = model.DateOfBirth;
                profile.Designation = model.Designation.ToUpper();
                profile.Employer = model.Employer.ToUpper();
                profile.EmployerAddress = model.EmployerAddress.ToUpper();
                profile.Email = email.ToUpper();
                profile.Gender = model.Gender;
                profile.Nationality = model.Nationality;
                profile.StateOfOrigin = model.StateOfOrigin;
                profile.LGA = model.LGA;
                profile.HomeTown = model.HomeTown.ToUpper();
                profile.MaritalStatus = model.MaritalStatus;
                profile.NextOfKin = model.NextofKin.ToUpper();
                profile.NokAddress = model.NoKAddress.ToUpper();
                profile.NoKPhoneNumber = model.NoKPhoneNumber;
                profile.NokRelationship = model.NoKRelationship.ToUpper();
                profile.Sector = model.Sector;
                profile.ProfileImage = profile.ID;
                profile.PhoneNumber = model.PhoneNumber;
                profile.Title = model.Title;
                profile.PostedBy = model.ActionBy.ToUpper();
                profile.TransDate = GetTransactionDate();
                profile.ApprovedBy = string.Empty;
                profile.Status = "PENDING";

                _context.Profile.Update(profile);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Falied"; }
        }

        public async Task<string> DropProfileAsync(string id)
        {
            try
            {
                var profile = _context.Profile.Where(b => b.ID == id).FirstOrDefault();
                if (profile != null)
                {
                    _context.Profile.Remove(profile);
                    await _context.SaveChangesAsync();
                }
                return "Succeeded";
            }
            catch { return "Falied"; }

        }

        public IEnumerable<Profile> GetIndividualProfile(string username)
        {
            //Return all the individual profile created by the specified user
            var prefix = _context.Parameter.Where(p => p.Name == "INDIVIDUAL_CUSTOMER_PREFIX").Select(p => p.Value).FirstOrDefault();
            return _context.Profile.Where(p => p.PostedBy == username && p.ID.Contains(prefix)).Include(p => p.Branch);
        }

        public IndividualProfileViewModel GetIndividualProfile(string id, int customer = 0)
        {
            //Return the individual profile by ID
            return (from profile in _context.Profile
                    where profile.ID == id
                    select new IndividualProfileViewModel
                    {
                         ID = profile.ID,
                        Lastname = profile.Lastname,
                        Othernames = profile.Othernames,
                        CustomerType = profile.CustomerType,
                        ContactAddress = profile.ContactAddress,
                        DateOfBirth = profile.DateOfBirth,
                        Designation = profile.Designation,
                        Employer = profile.Employer,
                        EmployerAddress = profile.EmployerAddress,
                        Email = profile.Email,
                        Gender = profile.Gender,
                        Nationality = profile.Nationality,
                        NextofKin = profile.NextOfKin,
                        NoKAddress = profile.NokAddress,
                        NoKPhoneNumber = profile.NoKPhoneNumber,
                        NoKRelationship = profile.NokRelationship,
                        PhoneNumber = profile.PhoneNumber,
                        StateOfOrigin = profile.StateOfOrigin,
                        LGA = profile.LGA,
                        HomeTown = profile.HomeTown,
                        MaritalStatus = profile.MaritalStatus,
                        Sector = profile.Sector,
                        Title = profile.Title,
                        StatusMessage = string.Empty
                    }).FirstOrDefault();

        }

        public string GetIndividualProfilePrefix()
        {
            return _context.Parameter.Where(p => p.Name == "INDIVIDUAL_CUSTOMER_PREFIX").Select(p => p.Value).FirstOrDefault();
        }

        #endregion

        //Handle corporate profile
        #region
        public Profile CheckIfProfileExist(string businessName, DateTime dateofIncorporation)
        {
            return _context.Profile.Where(p => p.Lastname == businessName && p.DateOfBirth == dateofIncorporation).FirstOrDefault();
        }

        public async Task<string> CreateCorporateProfileAsync(CorporateProfileViewModel model)
        {
            try
            {
                //This generates customer number for the user
                #region
                string customerNo = string.Empty;
                string website = string.Empty;
                string email = string.Empty;
                var branch = _context.ApplicationUser.Where(u => u.UserName == model.ActionBy)
                    .Include(b => b.Profile)
                    .Select(b => b.Profile.Branch)
                    .FirstOrDefault();

                if (model.CustomerType == "CORPORATE")
                {
                    customerNo = GenerateCustomerNo("CORPORATE_CUSTOMER_PREFIX");
                }
                else if (model.CustomerType == "COOPERATIVE")
                {
                    customerNo = GenerateCustomerNo("COOPERATIVE_CUSTOMER_PREFIX");
                }
                generatedCustomerNo = customerNo;


                if (model.Website == null)
                    website = string.Empty;
                else
                    website = model.Website;

                if (model.Email == null)
                    email = string.Empty;
                else
                    email = model.Email;

                #endregion

                Profile profile = new Profile
                {
                    ID = customerNo.ToUpper(),
                    Branch = branch,
                    ContactAddress = model.ContactAddress.ToUpper(),
                    CustomerBase = model.CustomerBase,
                    Lastname = model.CustomerName.ToUpper(),
                    CustomerType = model.CustomerType.ToUpper(),
                    DateOfBirth = model.DateOfIncorporation,
                    IncorporationType = model.IncorporationType.ToUpper(),
                    NextOfKin = model.PrincipalOfficer.ToUpper(),
                    RCNo = model.RCNo.ToUpper(),
                    RegisteredBody = model.RegisteredBody.ToUpper(),
                    StartupCapital = model.StartupCapital,
                    Turnover = model.Turnover,
                    Website = website.ToUpper(),
                    PhoneNumber = model.PhoneNumber,
                    Email = email.ToUpper(),
                    Sector = model.BusinessSector,
                    NokAddress = model.PrincipalOfficerAddress.ToUpper(),
                    NoKPhoneNumber = model.PrincipalOfficerPhoneNumber,
                    NokRelationship = model.PrincipalOfficerRelationship.ToUpper(),
                    ProfileImage = customerNo + ".jpg",
                    PostedBy = model.ActionBy.ToUpper(),
                    TransDate = GetTransactionDate(),
                    ApprovedBy = string.Empty,
                    Status = "PENDING"
                };

                _context.Profile.Add(profile);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Falied"; }
        }

        public async Task<string> UpdateCorporateProfileAsync(CorporateProfileViewModel model)
        {
            try
            {
                string website = string.Empty;

                if (model.Website == null)
                    website = string.Empty;
                else
                    website = model.Website;

                string email = string.Empty;
                if (model.Email == null)
                    email = string.Empty;
                else
                    email = model.Email;

                var profile = _context.Profile.Where(b => b.ID == model.ID).FirstOrDefault();

                profile.ContactAddress = model.ContactAddress.ToUpper();
                profile.CustomerBase = model.CustomerBase;
                profile.Lastname = model.CustomerName.ToUpper();
                profile.CustomerType = model.CustomerType.ToUpper();
                profile.DateOfBirth = model.DateOfIncorporation;
                profile.IncorporationType = model.IncorporationType.ToUpper();
                profile.RCNo = model.RCNo.ToUpper();
                profile.RegisteredBody = model.RegisteredBody.ToUpper();
                profile.StartupCapital = model.StartupCapital;
                profile.Turnover = model.Turnover;
                profile.Website = website.ToUpper();
                profile.NextOfKin = model.PrincipalOfficer.ToUpper();
                profile.NokAddress = model.PrincipalOfficerAddress.ToUpper();
                profile.NoKPhoneNumber = model.PrincipalOfficerPhoneNumber;
                profile.NokRelationship = model.PrincipalOfficerRelationship.ToUpper();
                profile.Email = email.ToUpper();
                profile.PhoneNumber = model.PhoneNumber;
                profile.Sector = model.BusinessSector;
                profile.ProfileImage = profile.ID;
                profile.PostedBy = model.ActionBy.ToUpper();
                profile.TransDate = GetTransactionDate();
                profile.ApprovedBy = string.Empty;
                profile.Status = "PENDING";

                _context.Profile.Update(profile);
                await _context.SaveChangesAsync();
                return "Succeeded";
            }
            catch { return "Falied"; }
        }

        public IEnumerable<Profile> GetCorporateProfile(string username)
        {
            //Return all the corporate profile created by the specified user
            var prefix = _context.Parameter.Where(p => p.Name == "CORPORATE_CUSTOMER_PREFIX").Select(p => p.Value).FirstOrDefault();
            return _context.Profile.Where(p => p.PostedBy == username && p.ID.Contains(prefix)).Include(p => p.Branch);
        }

        public CorporateProfileViewModel GetCorporateProfile(string id, int customer = 0)
        {
            //Return the corporate profile by ID
            return (from profile in _context.Profile
                    where profile.ID == id
                    select new CorporateProfileViewModel
                    {
                        ID = profile.ID,
                        ContactAddress = profile.ContactAddress,
                        CustomerBase = profile.CustomerBase,
                        CustomerName = profile.Lastname,
                        CustomerType = profile.CustomerType,
                        DateOfIncorporation = profile.DateOfBirth,
                        IncorporationType = profile.IncorporationType,
                        PrincipalOfficer = profile.NextOfKin,
                        PrincipalOfficerAddress = profile.NokAddress,
                        PrincipalOfficerPhoneNumber = profile.NoKPhoneNumber,
                        PrincipalOfficerRelationship = profile.NokRelationship,
                        PhoneNumber = profile.PhoneNumber,
                        Email = profile.Email,
                        RCNo = profile.RCNo,
                        RegisteredBody = profile.RegisteredBody,
                        StartupCapital = profile.StartupCapital,
                        Turnover = profile.Turnover,
                        Website = profile.Website,
                        BusinessSector = profile.Sector,
                        StatusMessage = string.Empty
                    }).FirstOrDefault();

        }
        #endregion
    }
}
