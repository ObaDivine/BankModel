using Microsoft.AspNetCore.Identity;
using System;

namespace BankModel.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string SecurityQuestion { get; set; }
        public string SecurityQuestionAnswer { get; set; }
        public DateTime PasswordExpiryDate { get; set; }
        public Profile Profile { get; set; }
        public decimal ApprovalLimit { get; set; }
        public decimal TransactionLimit { get; set; }
        public bool UserOnline { get; set; }
        public DateTime TransDate { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Status { get; set; }
    }
}
