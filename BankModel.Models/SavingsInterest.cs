using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class SavingsInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public Account Account { get; set; }
        public decimal Rate { get; set; }
        public decimal BalBF { get; set; }
        public decimal Amount { get; set; }
        public string TransMonth { get; set; }
        public string TransYear { get; set; }
        public DateTime TransDate { get; set; }
    }
}
