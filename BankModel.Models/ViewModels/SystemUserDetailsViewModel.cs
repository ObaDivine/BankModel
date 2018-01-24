using System;
using System.Collections.Generic;
using System.Text;

namespace BankModel.Models.ViewModels
{
    public class SystemUserDetailsViewModel
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string Branch { get; set; }
        public DateTime PasswordExpiryDate { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public decimal TransactionLimit { get; set; }
        public decimal ApprovalLimit { get; set; }
        public bool LockoutEnabled { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string StatusMessage { get; set; }

        //List all the roles of the user
        public List<string> Roles { get; set; }

        //List all the permissions of the user
        public List<string> Permissions { get; set; }
    }
}
