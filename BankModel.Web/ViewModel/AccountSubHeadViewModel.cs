using System.ComponentModel.DataAnnotations;


namespace BankModel.Web.ViewModels
{
    public class AccountSubHeadViewModel
    {

        [DataType(DataType.Text)]
        [Display(Name ="Account Code")]
        [StringLength(2)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account code required")]
        public string AccountCode { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Account Head")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account head required")]
        public string AccountHead { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Account Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account name required")]
        public string AccountName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Report Line")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Report line required")]
        public string ReportingLine { get; set; }

        public int ID { get; set; }
        public string StatusMessage { get; set; }
    }
}