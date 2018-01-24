using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace BankModel.Models.ViewModels
{
    public class CustomerAccountViewModel
    {
        [DataType(DataType.Text)]
        public string CustomerType { get; set; }

        [DataType(DataType.Text)]
        public string Customer { get; set; }

        [DataType(DataType.Text)]
        public string Product { get; set; }

        [DataType(DataType.Text)]
        public string StandingOrder { get; set; }

        public IFormFile AccountMandate { get; set; }

        [DataType(DataType.Text)]
        public string AccountOfficer { get; set; }

        public string ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }
    }
}