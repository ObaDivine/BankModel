using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BankModel.Models.ViewModels;
using System.Threading.Tasks;
using BankModel.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using BankModel.Models;
using BankModel.Service.Interfaces;

namespace BankModel.Web.Controllers
{
    [Authorize(Policy = "CashTransaction")]
    public class CashTransactionController : Controller
    {
        private readonly ICashRepository _cashRepository;
        private readonly ICashService _cashService;
        private readonly IValidationDictionary _validationDictionary;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public CashTransactionController(ICashRepository cashRepository, ICashService cashService, IValidationDictionary validationDictionary, IConfiguration config,
            UserManager<ApplicationUser> userManager)
        {
            _cashRepository = cashRepository;
            _cashService = cashService;
            _validationDictionary = validationDictionary;
            _userManager = userManager;
            _config = config;
        }

        [TempData]
        public string StatusMessage { get; set; }
        public string Result { get; set; }

        //Cash deposit transactions
        #region
        [HttpGet]
        [Authorize(Policy = "Deposit")]
        public IActionResult CashDeposit(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new TransactionViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CashDeposit(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = new TransactionViewModel { StatusMessage = "Please correct the errors" };
                //SetCustomerAccountViewItems();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var requirementResult = _cashService.ValidateCashDepositRequirement(model, user.UserName);
            if (requirementResult != null)
            {
                foreach (var error in _validationDictionary.GetValidationErrors())
                {
                    model = new TransactionViewModel { StatusMessage = error };
                    return View(model);
                }
            }

            //If here, then its a new branch
            Result = await _cashService.CreateCashDepositAsync(model, user.UserName);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(CashDeposit));
            }
            else
            {
                StatusMessage = "Error: Unable to create deposit transaction";
                return View(model);
            }
        }

        public async Task<IActionResult> CashDepositListing()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_cashRepository.GetCashDeposit(user.UserName));
        }

        #endregion

        //Cash withdrawal transactions
        #region
        [HttpGet]
        [Authorize(Policy = "Withdrawal")]
        public IActionResult CashWithdrawal(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new TransactionViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CashWithdrawal(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = new TransactionViewModel { StatusMessage = "Please correct the errors" };
                //SetCustomerAccountViewItems();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var requirementResult = _cashService.ValidateCashWithdrawalRequirement(model, user.UserName);
            if (requirementResult != null)
            {
                foreach (var error in _validationDictionary.GetValidationErrors())
                {
                    model = new TransactionViewModel { StatusMessage = error };
                    return View(model);
                }
            }

            //If here, then its a new branch
            Result = await _cashService.CreateCashWithdrawalAsync(model, user.UserName);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(CashWithdrawal));
            }
            else
            {
                StatusMessage = "Error: Unable to create withdrawal transaction";
                return View(model);
            }
        }

        public async Task<IActionResult> CashWithdrawalListing()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_cashRepository.GetCashWithdrawal(user.UserName));
        }
        #endregion


        //Fund transfer transactions
        #region
        [HttpGet]
        [Authorize(Policy = "FundTransfer")]
        public IActionResult FundTransfer(string returnUrl = null )
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new TransactionViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FundTransfer(TransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = new TransactionViewModel { StatusMessage = "Please correct the errors" };
                //SetCustomerAccountViewItems();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var requirementResult = _cashService.ValidateFundTransferRequirement(model, user.UserName);
            if (requirementResult != null)
            {
                foreach (var error in _validationDictionary.GetValidationErrors())
                {
                    model = new TransactionViewModel { StatusMessage = error };
                    return View(model);
                }
            }

           //If here, then its a new branch
            Result = await _cashService.CreateFundTransferAsync(model, user.UserName);
            if (Result.Equals("Succeeded"))
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(FundTransfer));
            }
            else
            {
                StatusMessage = "Error: Unable to create fund transfer transaction";
                return View(model);
            }
        }

        public async Task<IActionResult> FundTransferListing()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(_cashRepository.GetFundTransfer(user.UserName));
        }

        #endregion
    }
}
