using BankModel.Web.Interfaces;
using BankModel.Web.ViewModels;
using BankModel.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace BankModel.Web.Services
{
    public class TemplateService: ITemplateService
    {
        private readonly IValidationDictionary _validationDictionary;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private static HttpClient client = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();

        public TemplateService(IValidationDictionary validationDictionary, IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _validationDictionary = validationDictionary;
            _config = config;
            _userManager = userManager;
            client.BaseAddress = new System.Uri("https://localhost:44368/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public TemplateService(IValidationDictionary validationDictionary)
        {
            _validationDictionary = validationDictionary;
        }

        public async Task<bool> HasApprovalRole(string username)
        {
            //This is used to enable the approval button on account template listing. It determines if the user has Approval role
            //response = client.PostAsync()
            return false;
        }

        //Handle account template processes
        #region 
        public async Task<bool> IsAccountTemplateInUse(int id)
        {
            response = await client.GetAsync("api.bankmodel/controller/accounttemplateinuse");
            return (response.Content.ReadAsStringAsync().Result == "true" ? true : false);
        }

        protected async Task<bool> ValidateAccountTemplate(TemplateAccountViewModel model)
        {
            //Check if the template name already exist
            response = await client.GetAsync("api.bankmodel/controller/accounttemplateexist/" + model.TemplateName);
            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectExist"], " Template " + model.TemplateName));
            }

            //Check if the product code already exist
            response = await client.GetAsync("api.bankmodel/controller/productcodeexist/" + model.TemplateName);
            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("",string.Format(_config.GetSection("Messages")["ObjectExist"], " Product code " + model.ProductCode));
            }

            return _validationDictionary.IsValid;
        }

        public async Task<bool> CreateAccountTemplateAsync(TemplateAccountViewModel model)
        {
            var result = await ValidateAccountTemplate(model);
                
            if (!result)
            {
                return false;
            }

            string templateModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(templateModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/template/createaccounttemplate", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        protected async Task<bool> ValidateAccountTemplateUpdate(TemplateAccountViewModel model)
        {
            var templateName = await client.GetAsync("api.bankmodel/controller/accounttemplateexist/" + model.TemplateName);
            var productCode = await client.GetAsync("api.bankmodel/controller/productcodeexist/" + model.TemplateName);

            if (templateName.Content.ReadAsStringAsync().Result == "true" && productCode.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("",string.Format(_config.GetSection("Messages")["ObjectExist"], string.Concat("Branch code ", productCode, " or template ", templateName)));
            }

            return _validationDictionary.IsValid;
        }

        public async Task<bool> UpdateAccountTemplateAsync(TemplateAccountViewModel model)
        {
            var result = await ValidateAccountTemplateUpdate(model);
            if (!result)
            {
                return false;
            }

            string templateModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(templateModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PutAsync("api.bankmodel/template/updateaccounttemplate", contentData).Result;
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        protected async Task<bool> ValidateAccountTemplateDrop(int id)
        {
            response = await client.GetAsync("api.bankmodel/controller/accounttemplateinuse");
            if (response.Content.ReadAsStringAsync().Result == "true")
            {
                _validationDictionary.AddError("", string.Format(_config.GetSection("Messages")["ObjectInUse"], " Account template"));
            }
            return _validationDictionary.IsValid;
        }

        public async Task<bool> DropAccountTemplateAsync(int id)
        {
            var result = await ValidateAccountTemplateDrop(id);
            if (!result)
            {
                return false;
            }

            response = await client.DeleteAsync("api.bankmodel/template/dropaccounttemplate/" + id);
            return (response.StatusCode == System.Net.HttpStatusCode.OK ? true : false);
        }

        public async Task<IEnumerable<TemplateAccount>> GetAccountTemplateByUser(string username)
        {
            response = await client.GetAsync("api.bankmodel/template/accounttemplatebyuser/" + username);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<TemplateAccount>>(result);
        }

        public async Task<TemplateAccount> GetAccountTemplateDetails(int id)
        {
            response = await client.GetAsync("api.bankmodel/template/accounttemplatedetails/" + id);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<TemplateAccount>(result);
        }

        public async Task<TemplateAccountViewModel> GetAccountTemplateByID(int id)
        {
            response = await client.GetAsync("api.bankmodel/template/accounttemplatebyid/" + id);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<TemplateAccountViewModel>(result);
        }

        #endregion


    }
}
