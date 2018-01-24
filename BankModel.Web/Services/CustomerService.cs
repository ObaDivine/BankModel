using BankModel.Web.Interfaces;
using BankModel.Web.ViewModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BankModel.Web.Services
{
    public class CustomerService: ICustomerService
    {
        private readonly IValidationDictionary _validationDictionary;
        private readonly IConfiguration _config;
        private static HttpClient client = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();

        public CustomerService(IValidationDictionary validationDictionary, IConfiguration config)
        {
            _validationDictionary = validationDictionary;
            _config = config;
            client.BaseAddress = new System.Uri("https://localhost:44368/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public CustomerService(IValidationDictionary validationDictionary)
        {
            _validationDictionary = validationDictionary;
        }

        public async Task<bool> IsProfileInUse(string id)
        {
            response = await client.GetAsync("api.bankmodel/customerservice/profile-inuse/" + id);
            return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<bool> IsAdministrator(string username)
        {
            response = await client.GetAsync("api.bankmodel/customerservice/is-admin/" + username);
            return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
        }

        //Handle individual profile processes
        #region
        public async Task<bool> ValidateIndividualProfile(IndividualProfileViewModel model)
        {
            response = await client.GetAsync("api.bankmodel/customerservice/individual-profile-exist/" + model);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], string.Concat(model.Title, " ", model.Lastname, ", ", model.Othernames)));
            }

            response = await client.GetAsync("api.bankmodel/customerservice/phone-number-exist/" + model.PhoneNumber);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], string.Concat("Phone number ", model.PhoneNumber)));
            }

            response = await client.GetAsync("api.bankmodel/customerservice/email-exist/" + model.Email);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], string.Concat("Email address ", model.Email)));
            }

            return _validationDictionary.IsValid;
        }

        public async Task<bool> CreateIndividualProfileAsync(IndividualProfileViewModel model)
        {
            var result = await ValidateIndividualProfile(model);

            if (!result)
            {
                return false;
            }

            string profileModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(profileModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/customerservice/create-individual-profile", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<bool> ValidateProfileDrop(string id)
        {
            response = await client.GetAsync("api.bankmodel/customerservice/profile-inuse" + id);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectInUse"], " Profile"));
            }
            return _validationDictionary.IsValid;
        }

        public async Task<bool> DropProfileAsync(string id)
        {
            var result = await ValidateProfileDrop(id);

            if (!result)
            {
                return false;
            }

            response = client.DeleteAsync("api.bankmodel/customerservice/drop-profile/" + id).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<bool> UpdateIndividualProfileAsync(IndividualProfileViewModel model)
        {
            var result = await ValidateIndividualProfile(model);

            if (!result)
            {
                return false;
            }

            string profileModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(profileModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PutAsync("api.bankmodel/customerservice/update-individual-profile", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<string> GetIndividualProfilePrefix()
        {
            response = await client.GetAsync("api.bankmodel/customerservice/individual-prefix");
            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<string> GetGeneratedCustomerNo()
        {
            response = await client.GetAsync("api.bankmodel/customerservice/generated-customer-number");
            return response.Content.ReadAsStringAsync().Result;
        }

        #endregion

        //Handle corporate profile processes
        #region
        public async Task<bool> ValidateCorporateProfile(CorporateProfileViewModel model)
        {
            response = await client.GetAsync("api.bankmodel/customerservice/corporate-profile-exist/" + model);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], model.CustomerName));
            }

            response = await client.GetAsync("api.bankmodel/customerservice/phone-number-exist/" + model.PhoneNumber);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], string.Concat("Phone number ", model.PhoneNumber)));
            }

            response = await client.GetAsync("api.bankmodel/customerservice/email-exist/" + model.Email);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], string.Concat("Email address ", model.Email)));
            }

            return _validationDictionary.IsValid;
        }

        public async Task<bool> CreateCorporateProfileAsync(CorporateProfileViewModel model)
        {
            var result = await ValidateCorporateProfile(model);

            if (!result)
            {
                return false;
            }

            string profileModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(profileModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PutAsync("api.bankmodel/customerservice/update-corporate-profile", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<bool> UpdateCorporateProfileAsync(CorporateProfileViewModel model)
        {
            var result = await ValidateCorporateProfile(model);

            if (!result)
            {
                return false;
            }


            string profileModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(profileModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PutAsync("api.bankmodel/customerservice/update-corporate-profile", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<CorporateProfileViewModel> GetCorporateProfile(string id, int customer = 0)
        {
            response = await client.GetAsync("api.bankmodel/customerservice/get-corporate-profile/" + id);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<CorporateProfileViewModel>(result);
        }

        #endregion

        //Handle customer account processes
        #region
        public async Task<bool> IsAccountInUse(string id)
        {
            response = await client.GetAsync("api.bankmodel/customerservice/account-inuse/" + id);
            return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<bool> ValidateCustomerAccount(CustomerAccountViewModel model)
        {
            response = await client.GetAsync("api.bankmodel/customerservice/account-exist/" + model);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], string.Concat(model.Product, " for ", model.Customer)));
            }

            return _validationDictionary.IsValid;
        }

        public async Task<bool> ValidateCustomerAccountDrop(string id)
        {
            response = await client.GetAsync("api.bankmodel/customerservice/account-inuse" + id);
            if (Convert.ToBoolean(response.Content.ReadAsStringAsync().Result) == true)
            {
                _validationDictionary.AddError("", string.Concat(_config.GetSection("Messages")["ObjectInUse"], " Profile"));
            }
            return _validationDictionary.IsValid;
        }

        public async Task<bool> CreateCustomerAccountAsync(CustomerAccountViewModel model)
        {
            var result = await ValidateCustomerAccount(model);

            if (!result)
            {
                return false;
            }

            string accountModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(accountModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/customerservice/create-customer-account", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<bool> UpdateCustomerAccountAsync(CustomerAccountViewModel model)
        {
            var result = await ValidateCustomerAccount(model);

            if (!result)
            {
                return false;
            }

            string accountModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(accountModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PutAsync("api.bankmodel/customerservice/update-customer-account", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<bool> DropCustomerAccountAsync(string id)
        {
            var result = await ValidateCustomerAccountDrop(id);

            if (!result)
            {
                return false;
            }

            response = client.DeleteAsync("api.bankmodel/customerservice/drop-customer-account/" + id).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<string> GetGeneratedAccountNo()
        {
            response = await client.GetAsync("api.bankmodel/customerservice/generated-account-number");
            return response.Content.ReadAsStringAsync().Result;
        }

        #endregion
    }
}
