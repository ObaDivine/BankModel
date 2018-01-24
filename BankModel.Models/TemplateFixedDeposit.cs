using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class TemplateFixedDeposit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string TemplateName { get; set; }
        public string FixedDepositType { get; set; }
        public decimal Amount { get; set; }
        public int Tenor { get; set; }
        public decimal InterestRate { get; set; }
        public string InterestDropFrequency { get; set; }
        public bool ChargePenalty { get; set; }
        public string PenaltyChargeType { get; set; }
        public decimal PenaltyFee { get; set; }
        public decimal WitholdingTax { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
    }
}
