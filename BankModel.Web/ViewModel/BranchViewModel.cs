using System.ComponentModel.DataAnnotations;


namespace BankModel.Web.ViewModels
{
    public class BranchViewModel
    {

        [DataType(DataType.Text)]
        [Display(Name ="Branch Code")]
        [StringLength(2)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch code required")]
        public string BranchCode { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Branch Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch description required")]
        public string BranchDesc { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Branch Location")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch location required")]
        public string BranchLocation { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Branch Manager")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch manager required")]
        public string BranchManager { get; set; }

        public int ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }
    }
}