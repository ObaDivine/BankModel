using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class SMS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public Account Account { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public decimal Amount { get; set; }
        public string TransMonth { get; set; }
        public string TransYear { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
    }
}
