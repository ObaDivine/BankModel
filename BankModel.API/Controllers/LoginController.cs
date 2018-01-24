using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BankModel.Models.ViewModels;
using BankModel.Models;

namespace BankModel.API.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public LoginController(IConfiguration config, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [Route("api.bankmodel/login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            IActionResult response = Unauthorized();

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
                    var tokenString = GenerateToken(model);
                    response = Ok(tokenString);
                }
            }

            return response;
        }

        private string GenerateToken(LoginViewModel model)
        {

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, model.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, model.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtTokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["JwtTokens:Issuer"],
              _config["JwtTokens:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
