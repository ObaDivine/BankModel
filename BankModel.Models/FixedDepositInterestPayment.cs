using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class FixedDepositInterestPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public FixedDeposit FixedDeposit { get; set; }
        public Account Account { get; set; }
        public string FixedDepositType { get; set; }
        public decimal Interest { get; set; }
        public int PaymentMonth { get; set; }
        public int PaymentYear { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExecuted { get; set; }
        public string Status { get; set; }
    }
}
