using System.ComponentModel.DataAnnotations;

namespace BankModel.Models.ViewModels
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
