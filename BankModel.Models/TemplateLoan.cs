using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class TemplateLoan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string TemplateName { get; set; }
        //Get Loan types from the Chart of Accounts
        public string LoanClass { get; set; }
        //This is set to INDIVIDUAL or GROUP
        public string LoanType { get; set; }
        public decimal CeilingAmount { get; set; }
        public int Tenor { get; set; }
        public decimal InterestRate { get; set; }
        // Interest frequency are MONTHLY and WEEKLY
        public string InterestFrequency { get; set; }
        public string RepaymentProcessing { get; set; }
        //Interest type are FLAT, REDUCING BALANCE and LONE 
        public string InterestType { get; set; }
        public bool ChargeForLoanDefault { get; set; }
        //Loan Default fee can be 5% or 5000 as flat
        public string LoanDefaultFee { get; set; }
        public bool TerminateOnlyIfFunded { get; set; }
        public bool AutoTerminate { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
    }
}
