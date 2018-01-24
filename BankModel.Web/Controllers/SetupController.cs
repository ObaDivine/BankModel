using BankModel.Web.ViewModels;
using BankModel.Models;
using BankModel.Web.Services;
using BankModel.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankModel.Web.Controllers
{
    [Authorize(Policy = "Setup")]
    public class SetupController : Controller
    {
        private readonly ISetupService _setupService;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        [TempData]
        public string StatusMessage { get; set; }
        private bool Result;


        public SetupController(
            ISetupService setupService,
            IConfiguration config, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> singInManager)
        {
            _setupService = setupService;
            _config = config;
            _userManager = userManager;
            _signInManager = singInManager;
        }

        public SetupController()
        {
            _setupService = new SetupService(new ModelStateWrapper(this.ModelState));
        }

        //Branch details
        #region

        [HttpGet]
        [Authorize(Policy = "Branch")]
        [Authorize(Policy = "HeadOffice")]
        public IActionResult Branches()
        {
            var model = new BranchViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Branches(BranchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            model.ActionBy = user.UserName;
            Result = await _setupService.CreateBranchAsync(model);

            if (Result)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(Branches));
            }

            StatusMessage = "Error: Unable to create branch";
            return View(model);
        }

        [HttpGet]
        public IActionResult BranchListing()
        {
            return View(_setupService.GetBranchesWithDetails());
        }

        public IActionResult UpdateBranch(int ID)
        {
            var model = _setupService.GetBranchWithDetails(ID);
            return View(nameof(Branches), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBranch(BranchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            model.ActionBy = user.UserName;
            Result = await _setupService.UpdateBranchAsync(model);

            if (Result)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(Branches));
            }

            StatusMessage = "Error: Unable to update branch";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DropBranch(int ID)
        {
            Result =  await _setupService.DropBranchAsync(ID);
            if (Result)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(BranchListing));
            }
            else
            {
                StatusMessage = "Error: Unable to delete branch";
                return RedirectToAction(nameof(BranchListing));
            }
        }

        #endregion

        //Account sub head details
        #region

        [HttpGet]
        public IActionResult AccountSubHeadListing()
        {
            return View(_setupService.GetAccountSubHeads());
        }

        [HttpGet]
        [Authorize(Policy = "AccountSubHead")]
        [Authorize(Policy = "HeadOffice")]
        public IActionResult AccountSubHeads()
        {
            ViewData["AccountHead"] = new SelectList(new[] { "ASSET", "LIABILITY", "INCOME", "EXPENSE" });
            var model = new AccountSubHeadViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountSubHeads(AccountSubHeadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["AccountHead"] = new SelectList(new[] { "ASSET", "LIABILITY", "INCOME", "EXPENSE" });
                return View(model);
            }

            //If here, then its a new account sub head
            Result = await _setupService.CreateAccountSubHeadAsync(model);
            if (Result)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(AccountSubHeads));
            }

            StatusMessage = "Error: Unable to create account sub head";
            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateAccountSubHead(int id)
        {
            var model = _setupService.GetAccountSubHeads(id);
            ViewData["AccountHead"] = new SelectList(new[] { "ASSET", "LIABILITY", "INCOME", "EXPENSE" });
            return View(nameof(AccountSubHeads), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAccountSubHead(AccountSubHeadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["AccountHead"] = new SelectList(new[] { "ASSET", "LIABILITY", "INCOME", "EXPENSE" });
                return View(model);
            }

            //If here, then its a new account sub head
            Result = _setupService.UpdateAccountSubHeadAsync(model);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(AccountSubHeads));
            }

            StatusMessage = "Error: Unable to create account sub head";
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "AccountSubHead")]
        public async Task<IActionResult> DropAccountSubHead(int ID)
        {
            Result = await _setupService.DropAccountSubHeadAsync(ID);
            if (Result)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(AccountSubHeadListing));
            }

            StatusMessage = "Error: Unable to delete branch";
            return RedirectToAction(nameof(AccountSubHeadListing));
        }
        #endregion

    }
}
