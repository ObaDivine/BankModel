﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class FixedDepositFee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public FixedDeposit FixedDeposit { get; set; }
        public Account Account { get; set; }
        public string FeeType { get; set; }
        public string FeeTypeApplied { get; set; }
        public decimal AmountFixed { get; set; }
        public decimal AmountCharged { get; set; }
        public decimal FeesSubTotal { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
    }
}
