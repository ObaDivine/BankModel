using BankModel.Models;
using BankModel.Web.Interfaces;
using BankModel.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BankModel.Web.Services
{
    public class SystemAdminService : ISystemAdminService
    {
        private IValidationDictionary _validationDictionary;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient client = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();

        public SystemAdminService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
            client.BaseAddress = new System.Uri("https://localhost:44368/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public SystemAdminService(IValidationDictionary validationDictionary)
        {
            _validationDictionary = validationDictionary;
        }

        protected async Task<bool> ValidateSystemUser(SystemUsersViewModel model)
        {
            //Check if the proposed Username exist already
            var userResult = await _userManager.FindByNameAsync(model.Username.ToLower());
            if (userResult != null)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], model.Username));
            }

            return _validationDictionary.IsValid;
        }

        public async Task<bool> CreateSystemUserAsync(SystemUsersViewModel model)
        {
            var result = await ValidateSystemUser(model);

            if (!result)
            {
                return false;
            }

            string userModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(userModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/systemadmin/createsystemuser", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<bool> UpdateSystemUserAsync(SystemUsersViewModel model)
        {
            var result = await ValidateSystemUser(model);
            if (!result)
            {
                return false;
            }

            string userModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(userModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PutAsync("api.bankmodel/systemadmin/updatesystemuser", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        protected async Task<bool> ValidateSystemUserDrop(string id)
        {
            response = await client.GetAsync("api.bankmodel/systemadmin/systemuserinuse/" + id);
            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectInUse"], " System user"));
            }

            return _validationDictionary.IsValid;
        }

        public async Task<bool> DropSystemUserAsync(string id)
        {
            var result = await ValidateSystemUserDrop(id);
            if (!result)
            {
                return false;
            }

            response = await client.DeleteAsync("api.bankmodel/systemadmin/dropsystemuser/" + id);
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public bool MaintainSystemUser(SystemUsersViewModel model)
        {
            string userModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(userModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/systemadmin/maintainsystemuser", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<IEnumerable<string>> GetBranchStaff(string branch)
        {
            response = await client.GetAsync("api.bankmodel/systemadmin/branchstaff/" + branch);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<string>>(result);
        }

        public async Task<IEnumerable<SystemUserDetailsViewModel>> GetSystemUsers()
        {
            response = await client.GetAsync("api.bankmodel/systemadmin/systemusers");
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<SystemUserDetailsViewModel>>(result);
        }

        public async Task<SystemUsersViewModel> GetSystemUserWithDetails(string user)
        {
            response = await client.GetAsync("api.bankmodel/systemadmin/systemuserdetails/" + user);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<SystemUsersViewModel>(result);
        }

        public async Task<List<Parameter>> GetApplicationParameters()
        {
            response = await client.GetAsync("api.bankmodel/systemadmin/applicationparameters");
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<List<Parameter>>(result);
        }

        public async Task<SystemUserDetailsViewModel> GetUserDetails(string username)
        {
            response = await client.GetAsync("api.bankmodel/systemadmin/userdetails");
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<SystemUserDetailsViewModel>(result);
        }

    }
}
