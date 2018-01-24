using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class ProfitAndLoss
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public Branch Branch { get; set; }
        public string AccountType { get; set; }
        public string AccountSubType { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public decimal DRBalBF { get; set; }
        public decimal CRBalBF { get; set; }
        public decimal CurrentDR { get; set; }
        public decimal CurrentCR { get; set; }
        public decimal ClosingDRBal { get; set; }
        public decimal ClosingCRBal { get; set; }
        public int TransMonth { get; set; }
        public int TransYear { get; set; }
        public DateTime TransDate { get; set; }
    }
}
