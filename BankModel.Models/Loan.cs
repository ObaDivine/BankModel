using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public TemplateLoan Template { get; set; }
        //This is the customer account to be credited with the loan
        public Account Account { get; set; }
        public string LoanOfficer { get; set; }
        public Branch Branch { get; set; }
        //This is the account number of the Loan type in the Chart of Accounts
        public ChartOfAccount LoanAccount { get; set; }
        public string LoanReason { get; set; }
        public decimal AmountRequested { get; set; }
        public decimal AmountApproved { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime TerminationDate { get; set; }
        public DateTime ExtendedTerminationDate { get; set; }
        public bool ExecuteOnlyIfFunded { get; set; }
        public decimal LoanBalance { get; set; }
        public decimal TotalInterest { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }

        public ICollection<LoanFee> LoanFees { get; set; }
        public ICollection<LoanRepayment> LoanRepayments { get; set; }
    }
}
