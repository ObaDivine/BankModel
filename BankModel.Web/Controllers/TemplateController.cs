using BankModel.Web.Services;
using BankModel.Web.Interfaces;
using BankModel.Web.ViewModels;
using BankModel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace BankModel.Web.Controllers
{
    [Authorize(Policy = "Template")]
    public class TemplateController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        [TempData]
        public string StatusMessage { get; set; }
        private bool Result;

        public TemplateController(
            ITemplateService templateService,
            UserManager<ApplicationUser> userManager,
            IValidationDictionary validationDictionary,
            IConfiguration config)
        {
            _templateService = templateService;
            _config = config;
            _userManager = userManager;
        }

        public TemplateController()
        {
            _templateService = new TemplateService(new ModelStateWrapper(this.ModelState));
        }

        #region

        [HttpGet]
        [Authorize(Policy = "AccountTemplate")]
        public async Task<IActionResult> AccountTemplateListing()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_templateService.GetAccountTemplateByUser(user.UserName));
        }

        [Authorize(Policy = "AccountTemplate")]
        public IActionResult Account()
        {
            var model = new TemplateAccountViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "AccountTemplate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Account(TemplateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            model.ActionBy = user.UserName;
            Result = await _templateService.CreateAccountTemplateAsync(model);

            if (Result == true)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(Account));
            }

            model = new TemplateAccountViewModel { StatusMessage = "Error: Unable to create account template" };
            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateAccountTemplate(int ID)
        {
            var model = _templateService.GetAccountTemplateByID(ID);
            return View(nameof(Account), model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccountTemplate(TemplateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            model.ActionBy = user.UserName;
            Result = await _templateService.UpdateAccountTemplateAsync(model);

            if (Result == true)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(AccountTemplateListing));
            }

            model = new TemplateAccountViewModel { StatusMessage = "Error: Unable to create account template" };
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "AccountTemplate")]
        public async Task<IActionResult> DropAccountTemplate(int ID)
        {
            Result = await _templateService.DropAccountTemplateAsync(ID);

            if (Result == true)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(AccountTemplateListing));
            }

            StatusMessage = "Error: Unable to delete account template";
            return RedirectToAction(nameof(AccountTemplateListing));
        }

        [HttpGet]
        [Authorize(Policy = "AccountTemplate")]
        public IActionResult AccountTemplateDetails(int id)
        {
            return View(_templateService.GetAccountTemplateDetails(id));
        }

        #endregion

        #region
        [Authorize(Policy = "LoanTemplate")]
        public IActionResult Loan()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        #endregion

        #region
        [Authorize(Policy = "FixedDepositTemplate")]
        public IActionResult FixedDeposit()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        #endregion

    }
}
