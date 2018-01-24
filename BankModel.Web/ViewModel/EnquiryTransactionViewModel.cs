using System;
using System.ComponentModel.DataAnnotations;


namespace BankModel.Web.ViewModels
{
    public class EnquiryTransactionViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name ="ID")]
        public long ID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Transaction Date")]
        public DateTime TransDate { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Account")]
        public string Account { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Narration")]
        public string Narration { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Transaction Type")]
        public string TransType { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Balance BF")]
        public decimal BalanceBF { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Closing Balance")]
        public decimal ClosingBalance { get; set; }
    }
}