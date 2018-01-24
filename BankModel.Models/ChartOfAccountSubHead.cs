using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class ChartOfAccountSubHead
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string AccountHead { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string ReportingLine { get; set; }

        //Navigation
        public ICollection<ChartOfAccount> ChartOfAccount { get; set; }
    }
}
