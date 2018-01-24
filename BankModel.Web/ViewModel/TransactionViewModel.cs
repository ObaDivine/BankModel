using System.ComponentModel.DataAnnotations;


namespace BankModel.Web.ViewModels
{
    public class TransactionViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name ="Debit Account")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debit account required")]
        public string DR { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Credit Account")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Credit account required")]
        public string CR { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Transaction Narration")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Transaction narration required")]
        public string Narration { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Instrument Number")]
        public string InstrumentNo { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Amount")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Transaction amount required")]
        public decimal Amount { get; set; }

        public long ID { get; set; }
        public string StatusMessage { get; set; }
    }
}