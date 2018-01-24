using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankModel.Models
{
    public class BalanceSheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public Branch Branch { get; set; }
        public string AccountType { get; set; } // This is either Asset or Liability
        public string AccountSubType { get; set; } // E.g Current Asset, Contingent Liability
        public string AccountCode { get; set; } // 01, 02
        public string AccountDescription { get; set; } //E.g Bank Balances, Paid up Share Capitals
        public decimal CurrentBal { get; set; }
        public decimal LastMonthBal { get; set; }
        public decimal LastFinancialYearBal { get; set; }
        public decimal LastTwoFinancialYearBal { get; set; }
        public int TransMonth { get; set; }
        public int TransYear { get; set; }
        public DateTime TransDate { get; set; }
    }
}
