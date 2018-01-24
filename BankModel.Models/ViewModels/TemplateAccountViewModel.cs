using System.ComponentModel.DataAnnotations;


namespace BankModel.Models.ViewModels
{
    public class TemplateAccountViewModel
    {
        [DataType(DataType.Text)]
        public string TemplateName { get; set; }

        [DataType(DataType.Text)]
        public string ProductCode { get; set; }

        [DataType(DataType.Text)]
        public string AccountType { get; set; }

        [DataType(DataType.Text)]
        public decimal MinimumBalance { get; set; }

        [DataType(DataType.Text)]
        public decimal InterestPerAnnum { get; set; }

        [DataType(DataType.Text)]
        public string InterestDrop { get; set; }

        [DataType(DataType.Text)]
        public int WithdrawalTransactionLimit { get; set; }

        [DataType(DataType.Text)]
        public string WithdrawalTransactionPeriod { get; set; }

        public int WithdrawalAmountLimit { get; set; }

        [DataType(DataType.Text)]
        public string WithdrawalAmountPeriod { get; set; }

        [DataType(DataType.Text)]
        public int DepositTransactionLimit { get; set; }

        [DataType(DataType.Text)]
        public string DepositTransactionPeriod { get; set; }

        public int DepositAmountLimit { get; set; }

        [DataType(DataType.Text)]
        public string DepositAmountPeriod { get; set; }

        [DataType(DataType.Text)]
        public bool PostNoDebit { get; set; }

        [DataType(DataType.Text)]
        public bool PostNoCredit { get; set; }

        [DataType(DataType.Text)]
        public bool SMSNotification { get; set; }

        [DataType(DataType.Text)]
        public decimal SMSCost { get; set; }

        [DataType(DataType.Text)]
        public bool EmailNotification { get; set; }

        [DataType(DataType.Text)]
        public bool MonthlyStatement { get; set; }

        [DataType(DataType.Text)]
        public string MonthlyStatementBy { get; set; }

        [DataType(DataType.Text)]
        public decimal MonthlyStatementCost { get; set; }

        [DataType(DataType.Text)]
        public bool AllowOverdraw { get; set; }

        [DataType(DataType.Text)]
        public bool ChargeForOverdrawn { get; set; }

        [DataType(DataType.Text)]
        public string OverdrawnChargeType { get; set; }

        [DataType(DataType.Text)]
        public decimal OverdrawnFee { get; set; }

        [DataType(DataType.Text)]
        public bool UseForLoans { get; set; }

        [DataType(DataType.Text)]
        public bool UseForFixedDeposit { get; set; }

        [DataType(DataType.Text)]
        public bool AcceptCheques { get; set; }

        public int ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }

    }
}