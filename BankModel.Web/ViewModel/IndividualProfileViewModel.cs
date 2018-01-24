using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;


namespace BankModel.Web.ViewModels
{
    public class IndividualProfileViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Branch")]
        public string Branch { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Title")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title required")]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name required")]
        public string Lastname { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Other Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Other names required")]
        public string Othernames { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date of birth required")]
        public DateTime DateOfBirth { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Gender")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Gender required")]
        public string Gender { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Marital Status")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Marital status required")]
        public string MaritalStatus { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nationality")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nationality required")]
        public string Nationality { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "State of Origin")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "State of origin required")]
        public string StateOfOrigin { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Local Government Area")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Local Govt. Area required")]
        public string LGA { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Home Town")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Home town required")]
        public string HomeTown { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Phone number required")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Contact Address")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Contact address required")]
        public string ContactAddress { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Next of Kin")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Next of kin required")]
        public string NextofKin { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "NoK Address")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Next of kin address required")]
        public string NoKAddress { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nok Relationship")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Relationship with NoK required")]
        public string NoKRelationship { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "NoK Phone Number")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Next of Kin phone number required")]
        public string NoKPhoneNumber { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name of Employer")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employer required")]
        public string Employer { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Employer Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employer address required")]
        public string EmployerAddress { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Designation")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Designation required")]
        public string Designation { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Employer Sector")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Employer sector required")]
        public string Sector { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Customer Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Customer type required")]
        public string CustomerType { get; set; }

        [Display(Name = "Profile Image")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Profile image required")]
        public IFormFile ProfileImage { get; set; }

        public string ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }
    }
}