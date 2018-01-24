using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BankModel.Models.ViewModels;
using BankModel.Data.Interfaces;
using BankModel.Models;

namespace BankModel.Web.Controllers
{
    [Authorize]
    [Authorize(Policy = "Approval")]
    public class ApprovalController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApprovalRepository _approvalRepository;
        public ApprovalController(UserManager<ApplicationUser> userManager, IApprovalRepository approvalRepository)
        {
            _userManager = userManager;
            _approvalRepository = approvalRepository;
        }

        [TempData]
        public string StatusMessage { get; set; }

        //System user approval
        #region
        [HttpGet]
        [Authorize(Policy = "ApproveSystemUsers")]
        public async Task<IActionResult> SystemUsers()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user != null)
            {
                var pendingSystemUsers = _approvalRepository.GetPendingSystemUsers(user.UserName);
                var model = new SystemUserDetailsViewModel {StatusMessage = StatusMessage };
                return View(pendingSystemUsers);
            }
            return View();
        }


        public async Task<IActionResult> ApproveSystemUsers(string id)
        {
            var result = await _approvalRepository.ApproveSystemUsersAsync(id, _userManager.GetUserName(User));
            return RedirectToAction(nameof(SystemUsers));
        }
        #endregion

        //Branch approval
        #region
        [HttpGet]
        [Authorize(Policy = "ApproveBranch")]
        public async Task<IActionResult> Branches()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                var pendingBranches = _approvalRepository.GetPendingBranches(user.UserName);
                var model = new BranchViewModel { StatusMessage = StatusMessage };
                return View(pendingBranches);
            }
            return View();
        }

        public async Task<IActionResult> ApproveBranches(int id)
        {
            var result = await _approvalRepository.ApproveBranchesAsync(id, _userManager.GetUserName(User));
            return RedirectToAction(nameof(Branches));
        }
        #endregion

        //Chart of account approval
        #region
        [HttpGet]
        [Authorize(Policy = "ApproveChart")]
        public async Task<IActionResult> ChartofAccount()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                var pendingChartofAccount = _approvalRepository.GetPendingChartofAccount(user.UserName);
                var model = new BranchViewModel { StatusMessage = StatusMessage };
                return View(pendingChartofAccount);
            }
            return View();
        }

        [HttpGet]
        [Authorize(Policy = "ApproveChart")]
        public async Task<IActionResult> ApproveChartofAccount(int id)
        {
            var result = await _approvalRepository.ApproveChartofAccountAsync(id, _userManager.GetUserName(User));
            StatusMessage = "Chart of Account approved";
            return RedirectToAction(nameof(ChartofAccount));
        }
        #endregion

        //Template approval
        #region
        [HttpGet]
        [Authorize(Policy = "ApproveTemplate")]
        public async Task<IActionResult> Template(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                ViewBag.AccountTemplate = _approvalRepository.GetPendingAccountTemplate(user.UserName);
                ViewBag.LoanTemplate = _approvalRepository.GetPendingAccountTemplate(user.UserName);
                ViewBag.FixedDepositTemplate = _approvalRepository.GetPendingAccountTemplate(user.UserName);
                var model = new TemplateAccountViewModel { StatusMessage = StatusMessage };
                return View();
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ApproveAccountTemplate(int id)
        {
            var result = await _approvalRepository.ApproveAccountTemplateAsync(id, _userManager.GetUserName(User));
            return RedirectToAction(nameof(Template));
        }

        [HttpGet]
        [Authorize(Policy = "ApproveTemplate")]
        public IActionResult AccountTemplateDetails(int id)
        {
            return View(_approvalRepository.GetAccountTemplateDetails(id));
        }

        #endregion

        //Customer profile approval
        #region
        [HttpGet]
        [Authorize(Policy = "ApproveProfile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                var pendingProfile = _approvalRepository.GetPendingProfile(user.UserName);
                //var model = new SystemUserDetailsViewModel { StatusMessage = StatusMessage };
                return View(pendingProfile);
            }
            return View();
        }

        public async Task<IActionResult> ApproveProfile(string id)
        {
            var result = await _approvalRepository.ApproveProfileAsync(id, _userManager.GetUserName(User));
            return RedirectToAction(nameof(Profile));
        }
        #endregion

        //Customer account approval
        #region
        [Authorize(Policy = "ApproveAccount")]
        public async Task<IActionResult> CustomerAccount()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                var pendingCustomerAccount = _approvalRepository.GetPendingCustomerAccount(user.UserName);
                //var model = new SystemUserDetailsViewModel { StatusMessage = StatusMessage };
                return View(pendingCustomerAccount);
            }
            return View();
        }

        public async Task<IActionResult> ApproveCustomerAccount(string id)
        {
            var result = await _approvalRepository.ApproveCustomerAccountAsync(id, _userManager.GetUserName(User));
            return RedirectToAction(nameof(CustomerAccount));
        }
        #endregion
    }
}
