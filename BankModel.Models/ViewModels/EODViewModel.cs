using System;

namespace BankModel.Models.ViewModels
{
    public class EODViewModel
    {
        public DateTime StartDate { get; set; }
        public string StatusMessage { get; set; }
        public string EOMLastDaySavingsInterest { get; set; }
        public string EOMLastDayOverdrawn { get; set; }
        public string EOMSMS { get; set; }
        public string EOMLoanRepayment { get; set; }
        public string EOMLoanDefault { get; set; }
        public string EOMFixedDeposit { get; set; }
        public string EOMOverdrawn { get; set; }
        public string EOMProfitandLoss { get; set; }
        public string EOMBalanceSheet { get; set; }
        public string ActionBy { get; set; }
    }
}
