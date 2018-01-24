using BankModel.Web.ViewModels;
using BankModel.Models;
using BankModel.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using BankModel.Web.Services;

namespace BankModel.Web.Controllers
{
    public class GeneralLedgerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGeneralLedgerService _glService;
        private readonly IConfiguration _config;

        [TempData]
        public string StatusMessage { get; set; }
        private bool Result { get; set; }

        public GeneralLedgerController(
            UserManager<ApplicationUser> userManager,
            IGeneralLedgerService glService,
            IConfiguration config)
        {
            _userManager = userManager;
            _glService = glService;
            _config = config;
        }

        public GeneralLedgerController(IGeneralLedgerService glService)
        {
            _glService = new GeneralLedgerService(new ModelStateWrapper(this.ModelState));
        }



        //Chart of account details
        #region

        public JsonResult GetAccountSubHeads(string accountHead)
        {
            return Json(_glService.GetAccountSubHeads(accountHead));
        }

        [HttpGet]
        public async Task<IActionResult> ChartofAccountListing()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_glService.GetChartofAccountByUser(user.UserName));
        }

        [HttpGet]
        [Authorize(Policy = "ChartOfAccount")]
        public async Task<IActionResult> ChartofAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            var branches = await _glService.GetBranchNamesByUser(user.UserName);
            ViewData["Branches"] = new SelectList(branches);
            ViewData["AccountHead"] = new SelectList(new[] { "ASSET", "LIABILITY", "INCOME", "EXPENSE" });
            var model = new ChartofAccountViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChartofAccount(ChartofAccountViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                var branches = await _glService.GetBranchNamesByUser(user.UserName);
                ViewData["Branches"] = new SelectList(branches);
                ViewData["AccountHead"] = new SelectList(new[] { "ASSET", "LIABILITY", "INCOME", "EXPENSE" });
                return View(model);
            }

            //If here, then its a new chart of account item
            model.ActionBy = user.UserName;
            Result = await _glService.CreateChartofAccountAsync(model);
            if (Result)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(ChartofAccount));
            }

            StatusMessage = "Error: Unable to create account sub head";
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> UpdateChartofAccount(long id)
        {
            var user = await _userManager.GetUserAsync(User);
            var model = await _glService.GetChartofAccount(id);
            var branches = await _glService.GetBranchNamesByUser(user.UserName);
            ViewData["Branches"] = new SelectList(branches);
            ViewData["AccountHead"] = new SelectList(new[] { "ASSET", "LIABILITY", "INCOME", "EXPENSE" });
            return View(nameof(ChartofAccount), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateChartofAccount(ChartofAccountViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                var branches = await _glService.GetBranchNamesByUser(user.UserName);
                ViewData["Branches"] = new SelectList(branches);
                ViewData["AccountHead"] = new SelectList(new[] { "ASSET", "LIABILITY", "INCOME", "EXPENSE" });
                return View(model);
            }

            //If here then its an update
            model.ActionBy = user.UserName;
            Result = await _glService.UpdateChartofAccountAsync(model);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(ChartofAccount));
            }

            StatusMessage = "Error: Unable to update chart of account";
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "ChartOfAccount")]
        public async Task<IActionResult> DropChartofAccount(int ID)
        {
            Result = await _glService.DropChartofAccountAsync(ID);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(ChartofAccountListing));
            }

            StatusMessage = "Error: Unable to delete branch";
            return RedirectToAction(nameof(ChartofAccountListing));
        }

        #endregion


    }
}
