using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;


namespace BankModel.Models.ViewModels
{
    public class CorporateProfileViewModel
    {

        [DataType(DataType.Text)]
        public string CustomerName { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfIncorporation { get; set; }

        [DataType(DataType.Text)]
        public string IncorporationType { get; set; }

        [DataType(DataType.Text)]
        public string RegisteredBody { get; set; }

        [DataType(DataType.Text)]
        public string RCNo { get; set; }

        [DataType(DataType.Text)]
        public string ContactAddress { get; set; }

        [DataType(DataType.Text)]
        public int CustomerBase { get; set; }

        [DataType(DataType.Currency)]
        public decimal StartupCapital { get; set; }

        [DataType(DataType.Currency)]
        public decimal Turnover { get; set; }

        [DataType(DataType.Text)]
        public string BusinessSector { get; set; }

        [DataType(DataType.Text)]
        public string Website { get; set; }

        [DataType(DataType.Text)]
        public string CustomerType { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        public string PrincipalOfficer { get; set; }

        [DataType(DataType.Text)]
        public string PrincipalOfficerAddress { get; set; }

        [DataType(DataType.Text)]
        public string PrincipalOfficerRelationship { get; set; }

        [DataType(DataType.Text)]
        public string PrincipalOfficerPhoneNumber { get; set; }

        public IFormFile ProfileImage { get; set; }

        public string ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }

    }
}