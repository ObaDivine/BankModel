using BankModel.Models;
using BankModel.Web.Interfaces;
using BankModel.Web.Services;
using BankModel.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;


namespace BankModel.Web.Controllers
{
    [Authorize(Policy = "CustomerService")]
    //[Authorize(Policy = "HeadOffice")]
    public class CustomerServiceController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        [TempData]
        public string StatusMessage { get; set; }
        public bool Result { get; set; }

        public CustomerServiceController(
            ICustomerService customerService,
            IConfiguration config, 
            UserManager<ApplicationUser> userManager, 
            IHostingEnvironment hostingEnvironment)
        {
            _customerService = customerService;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _config = config;
        }

        public CustomerServiceController()
        {
            _customerService = new CustomerService(new ModelStateWrapper(this.ModelState));
        }

        //Handle customer accounts
        #region
        [HttpGet]
        [Authorize(Policy = "CustomerAccount")]
        public IActionResult AccountDetails(string id)
        {
            return View(_customerService.GetAccountDetails(id));
        }

        public async Task<JsonResult> GetCustomers(string customerType)
        {
            var user = await _userManager.GetUserAsync(User);
            return Json(_customerService.GetCustomers(customerType, user.UserName));
        }

        [HttpGet]
        [Authorize(Policy = "CustomerAccount")]
        public IActionResult CustomerAccount()
        {
            var model = new CustomerAccountViewModel { StatusMessage = StatusMessage };
            SetCustomerAccountViewItems();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CustomerAccount")]
        public async Task<IActionResult> CustomerAccount(CustomerAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //model = new CustomerAccountViewModel { StatusMessage = "Please correct the errors" };
                SetCustomerAccountViewItems();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            //If here, then its a new customer account
            model.ActionBy = user.UserName;
            Result = await _customerService.CreateCustomerAccountAsync(model);
            if (Result.Equals("Succeeded"))
            {
                //This gets the generated Account Number
                string accountNo = await _customerService.GetGeneratedAccountNo();

                var accountMandateFileName = Path.Combine(_hostingEnvironment.WebRootPath, "assets/images/signature-mandates/" + accountNo + ".jpg");
                using (var stream = new FileStream(accountMandateFileName, FileMode.Create))
                {
                    model.AccountMandate.CopyTo(stream);
                }

                StatusMessage = _config.GetSection("Messages")["Success"];
                SetCustomerAccountViewItems();
                return RedirectToAction(nameof(CustomerAccount));
            }

            model = new CustomerAccountViewModel { StatusMessage = "Error: Unable to create account" };
            SetCustomerAccountViewItems();
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "CustomerAccount")]
        public async Task<IActionResult> CustomerAccountListing()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_customerService.GetCustomerAccount(user.UserName));
        }

        [HttpGet]
        [Authorize(Policy = "CustomerAccount")]
        public async Task<IActionResult> DropCustomerAccount(string ID)
        {
            var requirementResult = _customerService.ValidateCustomerAccountDrop(ID);
            if (requirementResult != null)
            {
                foreach (var error in _validationDictionary.GetValidationErrors())
                {
                    StatusMessage = error;
                    return RedirectToAction(nameof(CustomerAccountListing));
                }
            }

            Result = await _customerService.DropCustomerAccountAsync(ID);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(CustomerAccountListing));
            }
            else
            {
                StatusMessage = "Error: Unable to delete account";
                return RedirectToAction(nameof(CustomerAccountListing));
            }
        }

        public IActionResult UpdateCustomerAccount(string ID)
        {
            SetCustomerAccountViewItems();
            var model = _customerService.GetCustomerAccount(ID,0);
            return View(nameof(CustomerAccount), model);
        }

        [HttpPost]
        public IActionResult UpdateCustomerAccount()
        {
            if (!string.IsNullOrEmpty(model.ID))
            {
                var updateRequirementResult = _customerService.ValidateCustomerAccountRequirement(model);
                if (updateRequirementResult != null)
                {
                    foreach (var error in _validationDictionary.GetValidationErrors())
                    {
                        model = new CustomerAccountViewModel { StatusMessage = error };
                        SetCustomerAccountViewItems();
                        return View(model);
                    }
                }

                //If here, then its individual profile update
                Result = await _customerService.UpdateCustomerAccountAsync(model);
                if (Result.Equals("Succeeded"))
                {
                    StatusMessage = _config.GetSection("Messages")["Success"];
                    SetCustomerAccountViewItems();
                    return RedirectToAction(nameof(CustomerAccountListing));
                }
                else
                {
                    model = new CustomerAccountViewModel { StatusMessage = "Error: Unable to update account" };
                    SetCustomerAccountViewItems();
                    return View(model);
                }
            }
        }

        private async void SetCustomerAccountViewItems()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Customers"] = new SelectList(new[] { "" });
            ViewData["AccountTemplate"] = new SelectList(_customerService.GetAccountTemplateNames());
            ViewData["AccountOfficer"] = new SelectList(_customerService.GetBranchStaff(user.UserName));
        }
        #endregion

        //Handle individual profile
        #region
        public JsonResult FillLGAs(string state)
        {
            return Json(_customerService.GetLGAs(state));
        }

        [HttpGet]
        [Authorize(Policy = "IndividualProfile")]
        public IActionResult ProfileDetails(string id)
        {
            return View(_customerService.GetProfileDetails(id));
        }

        [HttpGet]
        [Authorize(Policy = "IndividualProfile")]
        public IActionResult IndividualProfile()
        {
            var model = new IndividualProfileViewModel { StatusMessage = StatusMessage };
            SetIndividualProfileViewItems();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "IndividualProfile")]
        public async Task<IActionResult> IndividualProfile(IndividualProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                model = new IndividualProfileViewModel { StatusMessage = "Please correct the errors" };
                SetIndividualProfileViewItems();
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ID))
            {
                var updateRequirementResult = _customerService.ValidateIndividualProfileRequirement(model);
                if (updateRequirementResult != null)
                {
                    foreach (var error in _validationDictionary.GetValidationErrors())
                    {
                        model = new IndividualProfileViewModel { StatusMessage = error };
                        SetIndividualProfileViewItems();
                        return View(model);
                    }
                }

                //If here, then its individual profile update
                Result = await _customerService.UpdateIndividualProfileAsync(model);
                if (Result.Equals("Succeeded"))
                {
                    StatusMessage = _config.GetSection("Messages")["Success"];
                    return RedirectToAction(nameof(IndividualProfileListing));
                }
                else
                {
                    model = new IndividualProfileViewModel { StatusMessage = "Error: Unable to update profile" };
                    SetIndividualProfileViewItems();
                    return View(model);
                }
            }

            var requirementResult = _customerService.ValidateIndividualProfileRequirement(model);
            if (requirementResult != null)
            {
                foreach (var error in _validationDictionary.GetValidationErrors())
                {
                    model = new IndividualProfileViewModel { StatusMessage = error };
                    SetIndividualProfileViewItems();
                    return View(model);
                }
            }

            //If here, then its a new individual profile
            Result = await _customerService.CreateIndividualProfileAsync(model, user.UserName);
            if (Result.Equals("Succeeded"))
            {
                //This gets the generated Customer Number
                string customerNo = _customerService.GetGeneratedCustomerNo();

                var profileImageFileName = Path.Combine(_hostingEnvironment.WebRootPath, "assets/images/profile-images/" + customerNo + ".jpg");
                using (var stream = new FileStream(profileImageFileName, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(stream);
                }

                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(IndividualProfile));
            }
            else
            {
                model = new IndividualProfileViewModel { StatusMessage = "Error: Unable to create individual profile" };
                SetIndividualProfileViewItems();
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Policy = "IndividualProfile")]
        public async Task<IActionResult> IndividualProfileListing()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_customerService.GetIndividualProfile(user.UserName));
        }

        [HttpGet]
        [Authorize(Policy = "IndividualProfile")]
        public async Task<IActionResult> DropIndividualProfile(string ID)
        {
            var requirementResult = _customerService.ValidateIndividualProfileDrop(ID);
            if (requirementResult != null)
            {
                foreach (var error in _validationDictionary.GetValidationErrors())
                {
                    StatusMessage = error;
                    return RedirectToAction(nameof(IndividualProfileListing));
                }
            }

            Result = await _customerService.DropIndividualProfileAsync(ID);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(IndividualProfileListing));
            }
            else
            {
                StatusMessage = "Error: Unable to delete profile";
                return RedirectToAction(nameof(IndividualProfileListing));
            }
        }

        public IActionResult UpdateIndividualProfile(string ID)
        {
            SetIndividualProfileViewItems();
            var model = _customerService.GetIndividualProfile(ID,0);
            return View(nameof(IndividualProfile), model);
        }

        [HttpPost]
        public IActionResult UpdateIndividualProfile(IndividualProfileViewModel model)
        {

            //If here, then its individual profile update
            Result = await _customerService.UpdateCustomerAccountAsync(model);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                SetCustomerAccountViewItems();
                return RedirectToAction(nameof(CustomerAccountListing));
            }
            else
            {
                model = new CustomerAccountViewModel { StatusMessage = "Error: Unable to update account" };
                SetCustomerAccountViewItems();
                return View(model);
            }
        }

        private void SetIndividualProfileViewItems()
        {
            ViewData["Countries"] = new SelectList(new[] { "NIGERIA" });
            ViewData["States"] = new SelectList(_customerService.GetStates());
            ViewData["LGAs"] = new SelectList(_customerService.GetLGAs("BankModel"));
            ViewData["Branches"] = new SelectList(_customerService.GetBranchNames());
            #region
            ViewData["Sectors"] = new SelectList(new[] {"AQUACULTURE & MARICULTURE",
                                                        "AUTOMOBILES",
                                                        "BANKING",
                                                        "CHEMICALS",
                                                        "CLOTHING & TEXTILES",
                                                        "CONSTRUCTION & MATERIALS",
                                                        "CONTAINERS & PACKAGING",
                                                        "DELIVERY SERVICES/LOGISTICS",
                                                        "DEVELOPMENT FINANCE",
                                                        "EDUCATION",
                                                        "ELECTRONICS",
                                                        "ENERGY",
                                                        "ENVIRONMENT & WASTE",
                                                        "FORESTRY & PAPER",
                                                        "HEALTHCARE & PHARMACEUTICALS",
                                                        "INFORM & COMM TECHNOLOGY",
                                                        "INSURANCE",
                                                        "MANUFACTURING",
                                                        "MEDIA",
                                                        "MINING & METALS",
                                                        "OIL & GAS",
                                                        "PERSONAL & HOUSEHOLD GOODS",
                                                        "PUBLIC SECTOR ENTITY",
                                                        "RETAIL",
                                                        "SPORT",
                                                        "TELECOMMUNICATIONS",
                                                        "TOURISM & LEISURE",
                                                        "TRANSPORTATION",
                                                        "UTILITIES",
                                                        "OTHERS"
            });
            #endregion
        }

        #endregion

        //Handle corporate profile
        #region
        [HttpGet]
        [Authorize(Policy = "CorporateProfile")]
        public IActionResult CorporateProfile()
        {
            var model = new CorporateProfileViewModel { StatusMessage = StatusMessage };
            SetCorporateProfileViewItems();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CorporateProfile")]
        public async Task<IActionResult> CorporateProfile(CorporateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = new CorporateProfileViewModel { StatusMessage = "Please correct the errors" };
                SetCorporateProfileViewItems();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            //If here, then its a new individual profile
            model.ActionBy = user.UserName;
            Result = await _customerService.CreateCorporateProfileAsync(model);
            if (Result)
            {
                //This gets the generated Customer Number
                string customerNo = await _customerService.GetGeneratedCustomerNo();

                var profileImageFileName = Path.Combine(_hostingEnvironment.WebRootPath, "assets/images/profile-images/" + customerNo + ".jpg");
                using (var stream = new FileStream(profileImageFileName, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(stream);
                }
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(CorporateProfile));
            }

            model = new CorporateProfileViewModel { StatusMessage = "Error: Unable to create individual profile" };
            SetCorporateProfileViewItems();
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "CorporateProfile")]
        public async Task<IActionResult> CorporateProfileListing()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_customerService.GetCorporateProfile(user.UserName));
        }

        [HttpGet]
        [Authorize(Policy = "CorporateProfile")]
        public async Task<IActionResult> DropCorporateProfile(string ID)
        {
            Result = await _customerService.DropProfileAsync(ID);
            if (Result)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(CorporateProfileListing));
            }

            StatusMessage = "Error: Unable to delete profile";
            return RedirectToAction(nameof(CorporateProfileListing));
        }

        public IActionResult UpdateCorporateProfile(string ID)
        {
            SetCorporateProfileViewItems();
            var model = _customerService.GetCorporateProfile(ID,0);
            return View(nameof(CorporateProfile), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCorporateProfile(CorporateProfileViewModel model)
        {

            //If here, then its individual profile update
            Result = await _customerService.UpdateCorporateProfileAsync(model);
            if (Result)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                SetCustomerAccountViewItems();
                return RedirectToAction(nameof(CustomerAccountListing));
            }

            model = new CorporateProfileViewModel { StatusMessage = "Error: Unable to update profile" };
            SetCustomerAccountViewItems();
            return View(model);
        }

        private void SetCorporateProfileViewItems()
        {
            #region
            ViewData["Sectors"] = new SelectList(new[] {"AQUACULTURE & MARICULTURE",
                                                        "AUTOMOBILES",
                                                        "BANKING",
                                                        "CHEMICALS",
                                                        "CLOTHING & TEXTILES",
                                                        "CONSTRUCTION & MATERIALS",
                                                        "CONTAINERS & PACKAGING",
                                                        "DELIVERY SERVICES/LOGISTICS",
                                                        "DEVELOPMENT FINANCE",
                                                        "EDUCATION",
                                                        "ELECTRONICS",
                                                        "ENERGY",
                                                        "ENVIRONMENT & WASTE",
                                                        "FORESTRY & PAPER",
                                                        "HEALTHCARE & PHARMACEUTICALS",
                                                        "INFORM & COMM TECHNOLOGY",
                                                        "INSURANCE",
                                                        "MANUFACTURING",
                                                        "MEDIA",
                                                        "MINING & METALS",
                                                        "OIL & GAS",
                                                        "PERSONAL & HOUSEHOLD GOODS",
                                                        "PUBLIC SECTOR ENTITY",
                                                        "RETAIL",
                                                        "SPORT",
                                                        "TELECOMMUNICATIONS",
                                                        "TOURISM & LEISURE",
                                                        "TRANSPORTATION",
                                                        "UTILITIES",
                                                        "OTHERS"
            });
            #endregion
        }
        #endregion

    }
}
