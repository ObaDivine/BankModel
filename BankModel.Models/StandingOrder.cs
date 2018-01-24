using System;
using System.Collections.Generic;
using System.Text;

namespace BankModel.Models
{
    public class StandingOrder
    {
        public long ID { get; set; }
        public MonthlyFee Fees { get; set; }
        public Account Account { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
    }
}
