using BankModel.Web;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankModel.Web.ViewModels
{
    public class SystemUsersViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Staff Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Staff name required")]
        public string Staff { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username required")]
        public string Username { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Branch")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Branch required")]
        public string Branch { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Approval Ceiling")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Approval limit required")]
        public decimal ApprovalLimit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Transaction Ceiling")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Transaction limit required")]
        public decimal TransactionLimit { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Role")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "User role required")]
        public string Role { get; set; }

        public string ID { get; set; }

        [Display(Name = "Activate User")]
        public bool ActivateUser { get; set; }

        [Display(Name = "Maintenance Type")]
        public string MaintenanceType { get; set; }

        [Display(Name = "Password Days")]
        public int PasswordDays { get; set; }

        [Display(Name = "Limit")]
        public decimal LimitField { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "New Branch")]
        public string NewBranch { get; set; }

        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }
    }
}
