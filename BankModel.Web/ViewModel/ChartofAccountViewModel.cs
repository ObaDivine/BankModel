using System.ComponentModel.DataAnnotations;


namespace BankModel.Web.ViewModels
{
    public class ChartofAccountViewModel
    {

        [DataType(DataType.Text)]
        [Display(Name = "Account Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account name required")]
        public string AccountName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Account Head")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account head required")]
        public string AccountHead { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Account SubHead")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account sub head required")]
        public string AccountSubHead { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Branch")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch required")]
        public string Branch { get; set; }

        public long ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }
    }
}