using System.ComponentModel.DataAnnotations;


namespace BankModel.Web.ViewModels
{
    public class TemplateAccountViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name ="Template Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Template name required")]
        public string TemplateName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Product Code")]
        [StringLength(2)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Product code required")]
        public string ProductCode { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Account Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account type required")]
        public string AccountType { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Minimum Balance")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Minimum balance required")]
        public decimal MinimumBalance { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Interest Rate")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Interest rate per annum required")]
        public decimal InterestPerAnnum { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Interest Drop")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Interest drop frequency required")]
        public string InterestDrop { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Withdrawal Transaction")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Withdrawal restriction required")]
        public int WithdrawalTransactionLimit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Withdrawal Trans. Period")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Withdrawal restriction required")]
        public string WithdrawalTransactionPeriod { get; set; }

        [Display(Name ="Withdrawal Amount")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Withdrawal Amount limit required")]
        public int WithdrawalAmountLimit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Withdrawal Amount Period")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Withdrawal restriction required")]
        public string WithdrawalAmountPeriod { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Deposit Transaction")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Deposit restriction required")]
        public int DepositTransactionLimit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Deposit Trans. Period")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Deposit transaction required")]
        public string DepositTransactionPeriod { get; set; }

        [Display(Name = "Deposit Amount")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Deposit Amount limit required")]
        public int DepositAmountLimit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Deposit Amount Period")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Deposit restriction required")]
        public string DepositAmountPeriod { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Post No Debit")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Post no debit required")]
        public bool PostNoDebit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Post No Credit")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Post no credit required")]
        public bool PostNoCredit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="SMS Notification")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "SMS notification required")]
        public bool SMSNotification { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="SMS Cost")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "SMS cost required")]
        public decimal SMSCost { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Email Notification")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email notification required")]
        public bool EmailNotification { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Monthly Statement")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Monthly statement required")]
        public bool MonthlyStatement { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Monthly Statement By")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Monthly statement by required")]
        public string MonthlyStatementBy { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Statement Cost")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Monthly statement cost required")]
        public decimal MonthlyStatementCost { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Allow Overdraw")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Allow overdraw required")]
        public bool AllowOverdraw { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Charge Overdrawn")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Charge for overdrawn required")]
        public bool ChargeForOverdrawn { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Overdrawn Charge Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Overdrawn account charge type required")]
        public string OverdrawnChargeType { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Overdrawn Fee")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Overdrawn fee required")]
        public decimal OverdrawnFee { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Use For Loan")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Use for loan required")]
        public bool UseForLoans { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Use For Fixed Deposit")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Use for fixed deposit required")]
        public bool UseForFixedDeposit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Accept Cheques")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Accept cheques required")]
        public bool AcceptCheques { get; set; }

        public int ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }

    }
}