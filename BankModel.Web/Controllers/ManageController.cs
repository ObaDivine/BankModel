using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using BankModel.Models.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using BankModel.Models;
using Microsoft.Extensions.Configuration;

namespace BankModel.Web.Controllers
{
    public class ManageController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<ManageController> _logger;
        //private readonly IAuthorizationService _authorizationService;
        //private readonly IAccountService _accountService;

        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config,
            ILogger<ManageController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _logger = logger;
            //_authorizationService = authorizationService;
            //_accountService = accountService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //Check if the user password is set
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var passwordChageResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!passwordChageResult.Succeeded)
            {
                foreach (var error in passwordChageResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation(_config.GetSection("Messages")["PasswordUpdate"]);
            StatusMessage = _config.GetSection("Messages")["PasswordUpdate"];

            return RedirectToAction(nameof(ChangePassword));

        }

        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'."); ;

            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }
    }
}
