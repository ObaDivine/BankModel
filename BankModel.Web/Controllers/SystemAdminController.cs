using BankModel.Web.ViewModels;
using BankModel.Models;
using BankModel.Web.Interfaces;
using BankModel.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;

namespace BankModel.Web.Controllers
{
    [Authorize(Policy = "Administrator")]
    [Authorize(Policy = "HeadOffice")]
    public class SystemAdminController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISystemAdminService _systemAdminService;
        private readonly IConfiguration _config;
        private readonly IValidationDictionary _validationDictionary;
        private readonly ISetupService _setupService;

        [TempData]
        public string StatusMessage { get; set; }
        public bool Result { get; set; }

        public SystemAdminController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IConfiguration config, 
            RoleManager<IdentityRole> roleManager,
            ISystemAdminService systemAdminService,
            ISetupService setupService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _roleManager = roleManager;
            _setupService = setupService;
        }

        public SystemAdminController()
        {
            _systemAdminService = new SystemAdminService(new ModelStateWrapper(this.ModelState));
        }

        #region
        public JsonResult GetBranchStaff(string branch)
        {
            return Json(_systemAdminService.GetBranchStaff(branch));
        }

        [HttpGet]
        [Authorize(Policy = "SystemUsers")]
        public async Task<IActionResult> SystemUsers()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            ViewData["Branches"] = new SelectList(_setupService.GetBranchNames(user.UserName));
            ViewData["Roles"] = new SelectList(_roleManager.Roles);
            var branchStaff = await _systemAdminService.GetBranchStaff(string.Empty);
            ViewData["Staff"] = new SelectList(branchStaff);

            var model = new SystemUsersViewModel{ StatusMessage = StatusMessage};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SystemUsers(SystemUsersViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var user = await _userManager.GetUserAsync(User);
            var branchStaff = await _systemAdminService.GetBranchStaff(string.Empty);

            if (!ModelState.IsValid)
            {
                ViewData["Branches"] = new SelectList(_setupService.GetBranchNames(user.UserName));
                ViewData["Roles"] = new SelectList(_roleManager.Roles);
                ViewData["Staff"] = new SelectList(branchStaff);
                return View(model);
            }

            model.ActionBy = user.UserName;
            Result = await _systemAdminService.CreateSystemUserAsync(model);

            if (Result == true)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(SystemUsers));
            }

            model = new SystemUsersViewModel { StatusMessage = "Error: Unable to create user account" };
            ViewData["Branches"] = new SelectList(_setupService.GetBranchNames(user.UserName));
            ViewData["Roles"] = new SelectList(_roleManager.Roles);
            ViewData["Staff"] = new SelectList(branchStaff);
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "SystemUsers")]
        public IActionResult SystemUsersListing()
        {
            return View(_systemAdminService.GetSystemUsers());
        }

        public async Task<IActionResult> SystemUserDetails(string user)
        {
            var userDetails = await _systemAdminService.GetUserDetails(user);
            return View(userDetails);
        }

        [HttpGet]
        [Authorize(Policy = "SystemUsers")]
        public async Task<IActionResult> UpdateSystemUser(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Branches"] = new SelectList(_setupService.GetBranchNames(user.UserName));
            ViewData["Roles"] = new SelectList(_roleManager.Roles);
            var branchStaff = await _systemAdminService.GetBranchStaff(string.Empty);
            ViewData["Staff"] = new SelectList(branchStaff);
            var model = _systemAdminService.GetSystemUserWithDetails(id);
            return View(nameof(SystemUsers), model);
        }

        [HttpPost]
        [Authorize(Policy = "SystemUsers")]
        public async Task<IActionResult> UpdateSystemUser(SystemUsersViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var branchStaff = await _systemAdminService.GetBranchStaff(string.Empty);

            if (!ModelState.IsValid)
            {
                ViewData["Branches"] = new SelectList(_setupService.GetBranchNames(user.UserName));
                ViewData["Roles"] = new SelectList(_roleManager.Roles);
                ViewData["Staff"] = new SelectList(branchStaff);
                return View(model);
            }

            model.ActionBy = user.UserName;
            Result = await _systemAdminService.UpdateSystemUserAsync(model);

            if (Result == true)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(SystemUsersListing));
            }

            model = new SystemUsersViewModel { StatusMessage = "Error: Unable to update user account" };
            ViewData["Branches"] = new SelectList(_setupService.GetBranchNames(user.UserName));
            ViewData["Roles"] = new SelectList(_roleManager.Roles);
            ViewData["Staff"] = new SelectList(branchStaff);

            return View(model);
        }


        [HttpGet]
        [Authorize(Policy = "SystemUsers")]
        public async Task<IActionResult> DropSystemUser(string ID)
        {
            Result = await _systemAdminService.DropSystemUserAsync(ID);
            if (Result)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(SystemUsersListing));
            }

            StatusMessage = "Error: Unable to delete system user";
            return RedirectToAction(nameof(SystemUsersListing));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SystemUserMaintenance(SystemUsersViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var user = await _userManager.GetUserAsync(User);
            var branchStaff = await _systemAdminService.GetBranchStaff(string.Empty);

            if (!ModelState.IsValid)
            {
                ViewData["Branches"] = new SelectList(_setupService.GetBranchNames(user.UserName));
                ViewData["Roles"] = new SelectList(_roleManager.Roles);
                ViewData["Staff"] = new SelectList(branchStaff);
                return View(nameof(SystemUsers), model);
            }

            //Maintain the system user
            model.ActionBy = user.UserName;
            Result = _systemAdminService.MaintainSystemUser(model);

            if (Result == true)
            {
                StatusMessage = _config.GetSection("Messages")["Success"];
                return RedirectToAction(nameof(SystemUsers));
            }

            model = new SystemUsersViewModel { StatusMessage = "Error: Unable to maintain user account" };
            ViewData["Branches"] = new SelectList(_setupService.GetBranchNames(user.UserName));
            ViewData["Roles"] = new SelectList(_roleManager.Roles);
            ViewData["Staff"] = new SelectList(branchStaff);
            return View(nameof(SystemUsers), model);
        }

        #endregion

        [HttpGet]
        [Authorize(Policy = "Parameter")]
        public async Task<IActionResult> SystemParameters()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            return View(_systemAdminService.GetApplicationParameters());
        }


        #region
        public async Task<JsonResult> GetRolePermissions(string role)
        {
            //Get the identity role 
            var requestedRole = await _roleManager.FindByNameAsync(role);
            var rolePermissions = await _roleManager.GetClaimsAsync(requestedRole);
            return Json(rolePermissions.Select(c => c.Type));
        }

        [HttpGet]
        [Authorize(Policy = "UserRole")]
        public async Task<IActionResult> UserRoles()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            ViewData["Users"] = new SelectList(_userManager.Users.Select(u => u.NormalizedUserName));
            ViewData["Roles"] = new SelectList(_roleManager.Roles.Select(r => r.NormalizedName));

            var model = new UserRoleViewModel { StatusMessage = StatusMessage };
            return View(model);
        } 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(UserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await _roleManager.FindByNameAsync(model.Role);
            if(role != null)
            {
                var user = await _userManager.FindByNameAsync(model.User.ToLower());
                var result = await _userManager.AddToRoleAsync(user, role.Name);
            }

            StatusMessage = string.Concat(model.Role, " added to ", model.User);
            return RedirectToAction(nameof(UserRoles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(UserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await _roleManager.FindByNameAsync(model.Role);
            if (role != null)
            {
                var user = await _userManager.FindByNameAsync(model.User.ToLower());
                var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            }

            StatusMessage = string.Concat(model.Role, " removed from ", model.User);

            return RedirectToAction(nameof(UserRoles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRolePermission(UserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.User.ToLower());
            var selectedRole = await _roleManager.FindByNameAsync(model.Role);
            if(selectedRole != null)
            {
                //Get all the claims of the role
                var roleClaims = await _roleManager.GetClaimsAsync(selectedRole);
                if(roleClaims != null)
                {
                    //Get the selected claim 
                    var selectedClaim = roleClaims.Where(r => r.Type == model.Permission).FirstOrDefault();
                    //Check if the user already has the claim
                    var userClaims = await _userManager.GetClaimsAsync(user);
                    if (!userClaims.Contains(selectedClaim))
                    {
                        var result = await _userManager.AddClaimAsync(user, selectedClaim);
                        StatusMessage = string.Concat(model.Permission, " added to ", model.User);
                        return RedirectToAction(nameof(UserRoles));
                    }
                }
            }

           StatusMessage = string.Format(_config.GetSection("Messages")["ObjectNotFound"], model.Permission);

            return RedirectToAction(nameof(UserRoles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRolePermission(UserRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.User);
            if(user != null)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                if(userClaims != null)
                {
                    var selectedClaim = userClaims.Where(c => c.Type == model.Permission).FirstOrDefault();
                    if(selectedClaim != null)
                    {
                        var result = await _userManager.RemoveClaimAsync(user, selectedClaim);
                        StatusMessage = string.Concat(model.Permission, " removed from ", model.User);
                        return RedirectToAction(nameof(UserRoles));
                    }
                }
            }
            StatusMessage = string.Format(_config.GetSection("Messages")["ObjectNotFound"], model.Permission);

            return RedirectToAction(nameof(UserRoles));
        }

        #endregion

        #region

        #endregion
    }
}
