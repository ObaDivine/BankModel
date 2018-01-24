using BankModel.Models;
using BankModel.Web.Interfaces;
using BankModel.Web.ViewModels;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BankModel.Web.Services
{
    public class EnquiryService: IEnquiryService
    {
        private static HttpClient client = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();

        public EnquiryService()
        {
            client.BaseAddress = new System.Uri("https://localhost:44368/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<Account>> GetCustomerAccounts(string accountNo)
        {
            response = await client.GetAsync("apib.bankmodel/enquiry/customer-accounts/" + accountNo);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable<Account>>(result);
        }

        public async Task<EnquiryViewModel> GetCustomerAccountDetails(string accountNo)
        {
            response = await client.GetAsync("apib.bankmodel/enquiry/customer-accounts-details/" + accountNo);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<EnquiryViewModel>(result);
        }

        public async Task<Profile> GetCustomerProfile(string id)
        {
            response = await client.GetAsync("apib.bankmodel/enquiry/customer-profile/" + id);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<Profile>(result);
        }

        public async Task<IEnumerable> GetAccountTransactions(string accountNo)
        {
            response = await client.GetAsync("apib.bankmodel/enquiry/account-transactions/" + accountNo);
            string result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<IEnumerable>(result);
        }
    }
}
