using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;


namespace BankModel.Web.ViewModels
{
    public class CorporateProfileViewModel
    {

        [DataType(DataType.Text)]
        [Display(Name ="Business Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Business name required")]
        public string CustomerName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Date of Incorporation")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date of incorporation required")]
        public DateTime DateOfIncorporation { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Business Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Business type required")]
        public string IncorporationType { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Registerd Body")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Registered body required")]
        public string RegisteredBody { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="RC Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "RC number required")]
        public string RCNo { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Business Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Business address required")]
        public string ContactAddress { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Number of Customers")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Number of customers required")]
        public int CustomerBase { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Startup Capital")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Startup capital required")]
        public decimal StartupCapital { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Monthly Turnover")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Monthly turnover required")]
        public decimal Turnover { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Business Sector")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Business sector required")]
        public string BusinessSector { get; set; }

        [DataType(DataType.Text)]
        [Display(Name ="Company Website")]
        public string Website { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Customer Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Customer type required")]
        public string CustomerType { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone number required")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Company Email")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Email required")]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Principal Officer")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Principal officer required")]
        public string PrincipalOfficer { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Address")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Principal officer address required")]
        public string PrincipalOfficerAddress { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Relationship")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Relationship with principal officer required")]
        public string PrincipalOfficerRelationship { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Phone Number")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Principal officer phone number required")]
        public string PrincipalOfficerPhoneNumber { get; set; }

        [Display(Name = "Profile Image")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Image of principal officer required")]
        public IFormFile ProfileImage { get; set; }

        public string ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }

    }
}