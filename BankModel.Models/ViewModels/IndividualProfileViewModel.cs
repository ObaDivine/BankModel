using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;


namespace BankModel.Models.ViewModels
{
    public class IndividualProfileViewModel
    {
        [DataType(DataType.Text)]
        public string Branch { get; set; }

        [DataType(DataType.Text)]
        public string Title { get; set; }

        [DataType(DataType.Text)]
        public string Lastname { get; set; }

        [DataType(DataType.Text)]
        public string Othernames { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [DataType(DataType.Text)]
        public string MaritalStatus { get; set; }

        [DataType(DataType.Text)]
        public string Nationality { get; set; }

        [DataType(DataType.Text)]
        public string StateOfOrigin { get; set; }

        [DataType(DataType.Text)]
        public string LGA { get; set; }

        [DataType(DataType.Text)]
        public string HomeTown { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        public string ContactAddress { get; set; }

        [DataType(DataType.Text)]
        public string NextofKin { get; set; }

        [DataType(DataType.Text)]
        public string NoKAddress { get; set; }

        [DataType(DataType.Text)]
        public string NoKRelationship { get; set; }

        [DataType(DataType.Text)]
        public string NoKPhoneNumber { get; set; }

        [DataType(DataType.Text)]
        public string Employer { get; set; }

        [DataType(DataType.Text)]
        public string EmployerAddress { get; set; }

        [DataType(DataType.Text)]
        public string Designation { get; set; }

        [DataType(DataType.Text)]
        public string Sector { get; set; }

        [DataType(DataType.Text)]
        public string CustomerType { get; set; }

        public IFormFile ProfileImage { get; set; }
        public string ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }
    }
}