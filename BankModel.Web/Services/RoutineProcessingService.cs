using BankModel.Web.Interfaces;
using BankModel.Web.ViewModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BankModel.Web.Services
{
    public class RoutineProcessingService: IRoutineProcessingService
    {
        private readonly IValidationDictionary _validationDictionary;
        private readonly IConfiguration _config;
        private static HttpClient client = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();

        public RoutineProcessingService(IValidationDictionary validationDictionary, IConfiguration config)
        {
            _validationDictionary = validationDictionary;
            _config = config;
            client.BaseAddress = new System.Uri("https://localhost:44368/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public RoutineProcessingService(IValidationDictionary validationDictionary)
        {
            _validationDictionary = validationDictionary;
        }

        #region
        public async Task<int> GetPendingSystemUsers()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-users");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<int> GetPendingBranch()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-branch");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> GetPendingChartofAccount()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-chart-of-account");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> GetPendingProfile()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-profile");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> GetPendingAccount()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-account");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> GetPendingTransaction()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-transaction");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> GetPendingLoan()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-loan");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> GetPendingLoanRepayment()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-loan-repayment");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> GetPendingFixedDeposit()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-fixed-deposit");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> GetPendingSalary()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-salary");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        public async Task<int> GetPendingMobileMoney()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/pending-mobile-money");
            return Convert.ToInt16(response.Content.ReadAsStringAsync().Result);
        }
        #endregion

        public async Task<bool> NoEODPendingItems()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/eod-pending-items");
            return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
        }

        protected async Task<bool> ValidateEOD(DateTime startDate)
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/current-transaction-date");
            var currentDate = Convert.ToDateTime(response.Content.ReadAsStringAsync().Result);
            if (startDate <= Convert.ToDateTime(currentDate))
                _validationDictionary.AddError("", _config.GetSection("Messages")["InvalidStartDate"]);

            //Check that all the pending items are cleared
            var noPendingItems = await NoEODPendingItems();
            if (!noPendingItems)
                _validationDictionary.AddError("", _config.GetSection("Messages")["EODPendingItems"]);

            return _validationDictionary.IsValid;
             
        }

        public string EOD(EODViewModel model)
        {
            string eodModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routineprocessing/eod", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        #region
        public async Task<bool> IsEOMLastDay()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/eom-lastday");
            return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<bool> EOMLastDayCompleted()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/eom-lastday-completed");
            return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
        }

        public async Task<bool> EOMCompleted()
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/eom-completed");
            return Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
        }

        protected async Task<bool> ValidateEOMLastDay()
        {
            int lastDay = 0;
            response = await client.GetAsync("api.bankmodel/routineprocessing/current-transaction-date");

            lastDay = DateTime
                .DaysInMonth(Convert.ToDateTime(response.Content.ReadAsStringAsync().Result)
                .Year, Convert.ToDateTime(response.Content.ReadAsStringAsync().Result)
                .Month);
            if (Convert.ToDateTime(response.Content.ReadAsStringAsync().Result).Day != lastDay)
                _validationDictionary.AddError("", _config.GetSection("Messages")["NotLastDayofMonth"]);

            return _validationDictionary.IsValid;

        }

        protected async Task<bool> ValidateEOMLastDayCompletion()
        {
            var eomLastDayCompleted = await EOMLastDayCompleted();
            if (!eomLastDayCompleted)
                _validationDictionary.AddError("", _config.GetSection("Messages")["IncompleteLastDayTrans"]);

            return _validationDictionary.IsValid;

        }

        protected async Task<bool> ValidateStartofMonth(DateTime startDate)
        {
            response = await client.GetAsync("api.bankmodel/routine-processing/current-transaction-date");
            if (startDate <= Convert.ToDateTime(response.Content.ReadAsStringAsync().Result))
                _validationDictionary.AddError("", _config.GetSection("Messages")["InvalidStartDate"]);

            return _validationDictionary.IsValid;
        }

        public async Task<string> EOMLastDaySavingsInterest(string username)
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eomModel = JsonConvert.SerializeObject(username);
            var contentData = new StringContent(eomModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/eom-lastday-savings-interest", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMLastDayOverdrawn()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent("", System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/eom-lastday-overdrawn", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMSavingsInterest()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/eom-savings-interesta", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMOverdrawnAccount()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/eom-overdrawn-account", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMSMS()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/eom-sms", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMLoanRepayment()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/eom-loan-repayment", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMLoanDefault()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/eom-loan-default", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMFixedDeposit()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/eom-fixed-deposit", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMStandingOrder()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/eom-standing-order", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMProfitandLoss()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routine-processing/profit-and-loss", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> EOMBalanceSheet()
        {
            var result = await ValidateEOMLastDayCompletion();
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(string.Empty);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routineprocessing/balance-sheet", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        public async Task<string> StartofMonth(EODViewModel model)
        {
            var result = await ValidateStartofMonth(model.StartDate);
            if (!result)
            {
                return "Failed";
            }

            string eodModel = JsonConvert.SerializeObject(model);
            var contentData = new StringContent(eodModel, System.Text.Encoding.UTF8, "application/json");
            response = client.PostAsync("api.bankmodel/routineprocessing/start-of-month", contentData).Result;

            if (response.IsSuccessStatusCode)
            {
                return "Succeeded";
            }
            return string.Concat("Failed ", response.ReasonPhrase);
        }

        #endregion

        #region
        public async Task<bool> ValidateEOY(DateTime startDate)
        {
            response = await client.GetAsync("api.bankmodel/routineprocessing/current-transaction-date");
            if (startDate <= Convert.ToDateTime(response.Content.ReadAsStringAsync().Result))
                _validationDictionary.AddError("", _config.GetSection("Messages")["InvalidStartDate"]);

            return _validationDictionary.IsValid;

        }

        public async Task<string> EOY(EODViewModel model)
        {
            //return await _rpRepository.EOY(model);
            return string.Empty;
        }

        #endregion
    }
}
