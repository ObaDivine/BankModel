using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class LoanRepayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public Loan Loan { get; set; }
        public Account Account { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public int RepaymentMonth { get; set; }
        public int RepaymentYear { get; set; }
        //This is et to either AUTOMATIC or MANUAL
        public string RepaymentProcessing { get; set; }
        public Branch Branch { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateExecuted { get; set; }
        public bool ExecuteOnlyIfFunded { get; set; }
        public string PostedBy { get; set; }
        public string Status { get; set; }
    }
}
