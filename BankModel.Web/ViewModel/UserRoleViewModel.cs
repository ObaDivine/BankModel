using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BankModel.Web.ViewModels
{
    public class UserRoleViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "User is required")]
        public string User { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Role is required")]
        public string Role { get; set; }
        public string Permission { get; set; }
        public string StatusMessage { get; set; }
    }
}
