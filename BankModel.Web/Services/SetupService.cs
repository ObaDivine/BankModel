using BankModel.Web.ViewModels;
using BankModel.Web.Interfaces;
using BankModel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;

namespace BankModel.Web.Services
{
    public class SetupService : ISetupService
    {
        private IValidationDictionary _validationDictionary;
        private readonly IConfiguration _config;
        private static HttpClient client = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();

        public SetupService(IValidationDictionary validationDictionary, IConfiguration config)
        {
            _validationDictionary = validationDictionary;
            _config = config;
            client.BaseAddress = new System.Uri("https://localhost:44368/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public SetupService(IValidationDictionary validationDictionary)
        {
            _validationDictionary = validationDictionary;
        }

        //Handle Branch
        #region
        public async Task<bool> IsBranchInUse(int id)
        {
            response = await client.GetAsync("api.bankmodel/setup/branchinuse/" + id);
            return (response.Content.ReadAsStringAsync().Result == "true" ? true : false);
        }

        public async Task<IEnumerable<Branch>> GetBranchesWithDetails()
        {
            response = await client.GetAsync("api.bankmodel/setup/brancheswithdetails");
            var result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<Branch>>(result);
        }

        public async Task<BranchViewModel> GetBranchWithDetails(int id)
        {
            response = await client.GetAsync("api.bankmodel/setup/branchwithdetails/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<BranchViewModel>(result);
        }

        protected async Task<bool> ValidateBranch(BranchViewModel model)
        {
            response = await client.GetAsync("api.bankmodel/setup/branchcode/" + model.BranchCode);
            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], "Branch code" + model.BranchCode));
            }

            response = await client.GetAsync("api.bankmodel/setup/branchdescription/" + model.BranchDesc);
            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], "Branch name" + model.BranchDesc));
            }
            return _validationDictionary.IsValid;
        }

        public async Task<bool> CreateBranchAsync(BranchViewModel model)
        {
            var result = await ValidateBranch(model);
            if (!result)
            {
                return false;
            }

            string branchModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(branchModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/setup/createbranch", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        protected async Task<bool> ValidateBranchUpdate(BranchViewModel model)
        {
            var branchCode = await client.GetAsync("api.bankmodel/setup/branchdescription/" + model.BranchDesc);
            var branchDesc = await client.GetAsync("api.bankmodel/setup/branchdescription/" + model.BranchDesc);
            if (branchCode.Content.ReadAsStringAsync().Result == "true" && branchCode.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", 
                    string.Format(_config.GetSection("Messages")["ObjectExist"], 
                    string.Concat("Branch code ", model.BranchCode, " or branch description ", model.BranchDesc)));
            }

            return _validationDictionary.IsValid;
        }

        public async Task<bool> UpdateBranchAsync(BranchViewModel model)
        {
            var result = await ValidateBranchUpdate(model);
            if (!result)
            {
                return false;
            }

            string branchModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(branchModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PutAsync("api.bankmodel/setup/updatebranch", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<bool> ValidateBranchDrop(int id)
        {
            response = await client.GetAsync("api.bankmodel/setup/branchinuse/" + id);
            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectInUse"], " Branch"));
            }
            return _validationDictionary.IsValid;
        }

        public async Task<bool> DropBranchAsync(int id)
        {
            var result = await ValidateBranchDrop(id);
            if (!result)
            {
                return false;
            }

            response = client.DeleteAsync("api.bankmodel/setup/dropbranch/" + id).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<IEnumerable<string>> GetBranchNames(string username)
        {
            response = await client.GetAsync("api.bankmodel/setup/branches/" + username);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<string>>(result);
        }

        #endregion

        //Account sub heads
        #region
        public async Task<bool> IsAccountSubHeadInUse(int id)
        {
            response = await client.GetAsync("api.bankmodel/setup/accountsubheadinuse/" + id);
            return (response.Content.ReadAsStringAsync().Result == "true" ? true : false);
        }

        public async Task<IEnumerable<ChartOfAccountSubHead>> GetAccountSubHeads()
        {
            response = await client.GetAsync("api.bankmodel/setup/accountsubheads");
            var result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<ChartOfAccountSubHead>>(result);
        }

        public async Task<AccountSubHeadViewModel> GetAccountSubHeads(int id)
        {
            response = await client.GetAsync("api.bankmodel/setup/accountsubhead/" + id);
            var result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<AccountSubHeadViewModel>(result);
        }

        protected async Task<bool> ValidateAccountSubHead(AccountSubHeadViewModel model)
        {
            response = await client.GetAsync("api.bankmodel/setup/accountsubheadcode/"+ model.AccountCode + "/" + model.AccountHead);
            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("",string.Format(_config.GetSection("Messages")["ObjectExist"], "Account code " + model.AccountCode));
            }

            string accountModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(accountModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/setup/accountsubheadname", contentData).Result;

            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], "Account name " + model.AccountName));
            }
            return _validationDictionary.IsValid;
        }

        public async Task<bool> CreateAccountSubHeadAsync(AccountSubHeadViewModel model)
        {
            var result = await ValidateAccountSubHead(model);
            if (!result)
            {
                return false;
            }

            string accountModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(accountModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/setup/createaccountsubhead", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        protected bool ValidateAccountSubHeadUpdate(AccountSubHeadViewModel model)
        {
            string accountCodeModel = JsonConvert.SerializeObject(model);
            var accountCodeContentData = new StringContent(accountCodeModel, System.Text.Encoding.UTF8, "application/json");
            var accountCodeResponse = client.PostAsync("api.bankmodel/setup/accountsubheadcode", accountCodeContentData).Result;

            string accountNameModel = JsonConvert.SerializeObject(model);
            var accountNameContentData = new StringContent(accountNameModel, System.Text.Encoding.UTF8, "application/json");
            var accountNameResponse = client.PostAsync("api.bankmodel/setup/accountsubheadname", accountNameContentData).Result;

            if (accountCodeResponse.Content.ReadAsStringAsync().Result == "true" && accountNameResponse.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], string.Concat("Account code ", model.AccountCode, " or Account name ", model.AccountName)));
            }

            return _validationDictionary.IsValid;
        }

        public bool UpdateAccountSubHeadAsync(AccountSubHeadViewModel model)
        {
            if (!ValidateAccountSubHeadUpdate(model))
            {
                return false;
            }

            string accountModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(accountModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PutAsync("api.bankmodel/setup/updateaccountsubhead", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        protected async Task<bool> ValidateAccountSubHeadDrop(int id)
        {
            response = await client.GetAsync("api.bankmodel/setup/accountsubheadinuse/" + id);
            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectInUse"], " Account SubHead"));
            }
            return _validationDictionary.IsValid;
        }

        public async Task<bool> DropAccountSubHeadAsync(int id)
        {
            var result = await ValidateAccountSubHeadDrop(id);
            if (!result)
            {
                return false;
            }

            response = client.DeleteAsync("api.bankmodel/setup/dropaccountsubhead/"+ id).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        #endregion

    }
}
