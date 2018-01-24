using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text;

namespace BankModel.Models
{
    public class ChartOfAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public ChartOfAccountSubHead AccountSubHead { get; set; }
        public Branch Branch { get; set; }
        public decimal BookBalance { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
    }
}
