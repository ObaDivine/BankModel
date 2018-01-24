using System;
using System.Collections.Generic;
using System.Text;

namespace BankModel.Web.ViewModels
{
    public class EnquiryViewModel
    {
        public string ID { get; set; }
        public string Profile { get; set; }
        public string Branch { get; set; }
        public string AccountType { get; set; }
        public string ProductCode { get; set; }
        public decimal BookBalance { get; set; }
        public string StandingOrder { get; set; }
        public string AccountMandate { get; set; }
        public string AccountOfficer { get; set; }
        public decimal MinimumBalance { get; set; }
        public decimal InterestPerAnnum { get; set; }
        public string InterestDrop { get; set; }
        public int WithdrawalTransactionLimit { get; set; }
        public string WithdrawalTransactionPeriod { get; set; }
        public int WithdrawalAmountLimit { get; set; }
        public string WithdrawalAmountPeriod { get; set; }
        public int DepositTransactionLimit { get; set; }
        public string DepositTransactionPeriod { get; set; }
        public int DepositAmountLimit { get; set; }
        public string DepositAmountPeriod { get; set; }
        public bool PostNoDebit { get; set; }
        public bool PostNoCredit { get; set; }
        public bool SMSNotification { get; set; }
        public decimal SMSCost { get; set; }
        public bool EmailNotification { get; set; }
        public bool MonthlyStatement { get; set; }
        public string MonthlyStatementBy { get; set; }
        public decimal MonthlyStatementCost { get; set; }
        public bool ChargeForOverdrawn { get; set; }
        public string OverdrawnChargeType { get; set; }
        public decimal OverdrawnFee { get; set; }
        public bool UseForLoans { get; set; }
        public bool UseForFixedDeposit { get; set; }
        public bool AcceptCheques { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
    }
}
