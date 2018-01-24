using BankModel.Models;
using BankModel.Web.ViewModels;
using BankModel.Web.Services;
using BankModel.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace BankModel.Web.Controllers
{
    [Authorize(Policy = "RoutineProcessing")]
    public class RoutineProcessingController : Controller
    {
        private readonly IRoutineProcessingService _rpService;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private string Result;

        public RoutineProcessingController(IRoutineProcessingService rpService, IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _rpService = rpService;
            _config = config;
            _userManager = userManager;
        }

        public RoutineProcessingController()
        {
            _rpService = new RoutineProcessingService(new ModelStateWrapper(this.ModelState));
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        [Authorize(Policy = "EOD")]
        public IActionResult EOD()
        {
            ViewData["PendingSystemUsers"] = _rpService.GetPendingSystemUsers();
            ViewData["PendingBranch"] = _rpService.GetPendingBranch();
            ViewData["PendingChartofAccount"] = _rpService.GetPendingChartofAccount();
            ViewData["PendingProfile"] = _rpService.GetPendingProfile();
            ViewData["PendingAccount"] = _rpService.GetPendingAccount();
            ViewData["PendingTransaction"] = _rpService.GetPendingTransaction();
            ViewData["PendingLoan"] = _rpService.GetPendingLoan();
            ViewData["PendingLoanRepayment"] = _rpService.GetPendingLoanRepayment();
            ViewData["PendingFixedDeposit"] = _rpService.GetPendingFixedDeposit();
            ViewData["PendingSalary"] = _rpService.GetPendingSalary();
            ViewData["PendingMobileMoney"] = _rpService.GetPendingMobileMoney();

            var model = new EODViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EOD(EODViewModel model)
        {
            //Retrieve currnet signed in user
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                model.StatusMessage = "Please correct EOD errors in the model";
                return View(model);
            }

            model.ActionBy = user.UserName;
            Result = _rpService.EOD(model);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOD));
            }

            StatusMessage = Result;
            return RedirectToAction(nameof(EOD));

        }

        #region
        [HttpGet]
        [Authorize(Policy = "EOM")]
        public IActionResult EOM()
        {
            var model = new EODViewModel {StatusMessage = StatusMessage};
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> LastDayEOD(EODViewModel model)
        {
            //Retrieve currnet signed in user
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                model.StatusMessage = "Please correct EOD errors in the model";
                return View(model);
            }

            Result = await _rpService.EOMLastDaySavingsInterest(user.UserName);
            if (Result.Equals("Failed"))
            {
                StatusMessage = "Error: EOM last day savings interest uncompleted";
                return RedirectToAction(nameof(EOM));
            }

            Result = await _rpService.EOMLastDayOverdrawn();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: EOM last day overdrawn account uncompleted";
            return RedirectToAction(nameof(EOM));
        }

        [HttpGet]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> EOMSavingsInterest()
        {
            Result = await _rpService.EOMSavingsInterest();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));
        }

        [HttpGet]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> EOMOverdrawnAccount()
        {

            Result = await _rpService.EOMOverdrawnAccount();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));
        }


        [HttpGet]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> EOMSMS()
        {

            Result = await _rpService.EOMSMS();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EOMLoanRepayment()
        {

            Result = await _rpService.EOMLoanRepayment();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));
        }

        [HttpGet]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> EOMLoanDefault()
        {
            Result = await _rpService.EOMLoanDefault();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));
        }

        [HttpGet]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> EOMFixedDeposit()
        {

            Result = await _rpService.EOMFixedDeposit();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));
        }

        [HttpGet]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> EOMStandingOrder()
        {

            Result = await _rpService.EOMStandingOrder();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));
        }

        [HttpGet]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> EOMProfitandLoss()
        {
            Result = await _rpService.EOMProfitandLoss();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));
        }

        [HttpGet]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> EOMBalanceSheet(EODViewModel model)
        {
            //Retrieve currnet signed in user
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Result = await _rpService.EOMBalanceSheet();
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));

        }

        [HttpPost]
        [Authorize(Policy = "EOM")]
        public async Task<IActionResult> StartofMonth(EODViewModel model)
        {
            //Retrieve currnet signed in user
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.ActionBy = user.UserName;
            Result = await _rpService.StartofMonth(model);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOM));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOM));
        }

        #endregion


        [HttpGet]
        [Authorize(Policy = "EOY")]
        public IActionResult EOY()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EOY(EODViewModel model)
        {
            //Retrieve currnet signed in user
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.ActionBy = user.UserName;
            Result = await _rpService.EOY(model);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(EOY));
            }

            StatusMessage = "Error: Unable to end day";
            return RedirectToAction(nameof(EOY));
        }

    }
}
