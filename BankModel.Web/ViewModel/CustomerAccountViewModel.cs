using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace BankModel.Web.ViewModels
{
    public class CustomerAccountViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Customer Type")]
        public string CustomerType { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Customer")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Customer required")]
        public string Customer { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Product")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Product required")]
        public string Product { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Standing Order")]
        public string StandingOrder { get; set; }

        [Display(Name ="Customer Mandate")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account mandate required")]
        public IFormFile AccountMandate { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Account Officer")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account officer required")]
        public string AccountOfficer { get; set; }

        public string ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }
    }
}