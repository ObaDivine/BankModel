using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class GLTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string TransType { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public string DR { get; set; }
        public decimal DRBalBF { get; set; }
        public decimal DRBal { get; set; }
        public string CR { get; set; }
        public decimal CRBalBF { get; set; }
        public decimal CRBal { get; set; }
        public string InstrumentNo { get; set; }
        public string RefNo { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
    }
}
