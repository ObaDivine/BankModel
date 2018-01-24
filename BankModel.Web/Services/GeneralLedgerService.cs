using BankModel.Web.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankModel.Web.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using Newtonsoft.Json;

namespace BankModel.Web.Services
{
    public class GeneralLedgerService: IGeneralLedgerService
    {

        private readonly IValidationDictionary _validationDictionary;
        private readonly IConfiguration _config;
        private static HttpClient client = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();

        public GeneralLedgerService(IValidationDictionary validationDictionary, IConfiguration config)
        {
            _config = config;
            client.BaseAddress = new System.Uri("https://localhost:44368/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public GeneralLedgerService(IValidationDictionary validationDictionary)
        {
            _validationDictionary = validationDictionary;
        }

        //Chart of account
        #region
        public async Task<bool> IsChartofAccountInUse(long id)
        {
            response = await client.GetAsync("api.bankmodel/generalledger/gl-account-inuse/" + id);
            return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<bool> ValidateChartofAccount(ChartofAccountViewModel model)
        {
            response = await client.GetAsync("api.bankmodel/generalledger/gl-account-exist/" + model);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], "Chart item "));
            }

            //If the account name contains TELLER, validate if the username is valid
            if (model.AccountName.ToUpper().Contains("TELLER"))
            {
                var username = GetUsernameFromAccountName(model.AccountName);
                response = await client.GetAsync("api.bankmodel/generalledger/username-exist/" + model.AccountName);
                if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == false)
                {
                    _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectNotFound"], username, " as username "));
                }
            }

            return _validationDictionary.IsValid;
        }

        public async Task<bool> CreateChartofAccountAsync(ChartofAccountViewModel model)
        {
            var result = await ValidateChartofAccount(model);
            if (!result)
            {
                return false;
            }

            string accountModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(accountModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/generalledger/create-gl-account", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<bool> UpdateChartofAccountAsync(ChartofAccountViewModel model)
        {
            var result = await ValidateChartofAccount(model);
            if (!result)
            {
                return false;
            }

            string accountModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(accountModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PutAsync("api.bankmodel/generalledger/create-gl-account", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<bool> ValidateChartofAccountDrop(int id)
        {
            response = await client.GetAsync("api.bankmodel/generalledger/gl-account-inuse/" + id);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectInUse"], " Chart item"));
            }
            return _validationDictionary.IsValid;
        }

        public async Task<bool> DropChartofAccountAsync(int id)
        {
            var result = await ValidateChartofAccountDrop(id);
            if (!result)
            {
                return false;
            }

            response = client.DeleteAsync("api.bankmodel/generalledger/drop-gl-account/" + id).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        private string GetUsernameFromAccountName(string accountName)
        {
            return accountName.Substring(7, (accountName.Length - 7));
        }

        public async Task<IEnumerable<string>> GetAccountSubHeads(string accountHead)
        {
            response = await client.GetAsync("api.bankmodel/generalledger/account-subheads/" + accountHead);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<string>>(result);
        }

        public async Task<IEnumerable<BranchChartofAccountViewModel>> GetChartofAccountByUser(string username)
        {
            response = await client.GetAsync("api.bankmodel/generalledger/gl-account-by-user/" + username);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<BranchChartofAccountViewModel>>(result);
        }

        public async Task<IEnumerable<string>> GetBranchNamesByUser(string username)
        {
            response = await client.GetAsync("api.bankmodel/generalledger/branches-by-user/" + username);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<string>>(result);
        }

        public async Task<ChartofAccountViewModel> GetChartofAccount(long id)
        {
            response = await client.GetAsync("api.bankmodel/generalledger/gl-account-by-id/" + id);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<ChartofAccountViewModel>(result);
        }

        #endregion
    }
}
