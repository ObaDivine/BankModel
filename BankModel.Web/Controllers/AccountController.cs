using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BankModel.Service.Interfaces;
using BankModel.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace BankModel.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAccountService _accountService;
        private readonly IValidationDictionary _validationDictionary;
        private static HttpClient client = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config,
            ILogger<AccountController> logger, IAuthorizationService authorizationService, IAccountService accountService, IValidationDictionary validationDictionary)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _logger = logger;
            _authorizationService = authorizationService;
            _accountService = accountService;
            _validationDictionary = validationDictionary;
            client.BaseAddress = new System.Uri("https://localhost:44368/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [TempData]
        public string StatusMessage {get;set;}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            var model = new LoginViewModel { StatusMessage = StatusMessage };
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            string loginModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(loginModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/login", contentData).Result;
            var responsemessage  = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);
                _accountService.SetUserLogin(model.Username);
            }

            ModelState.Clear();
            ModelState.AddModelError("", "Username or Password incorrect");
            return View(model);

        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            var user = await _userManager.GetUserAsync(User);
            _accountService.ClearUserLogin(user.UserName);
            _logger.LogInformation("Signed out");
            return RedirectToAction(nameof(Login));
        }

        #region
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        #endregion
    }
}