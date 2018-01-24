using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BankModel.Models
{
    public class Profile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }
        //This is Individual, Corporate or Cooperative
        public string CustomerType { get; set; }
        public string Title { get; set; }
        public string Lastname { get; set; }
        public string Othernames { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Nationality { get; set; }
        public string StateOfOrigin { get; set; }
        public string LGA { get; set; }
        public string HomeTown { get; set; }
        public string Employer { get; set; }
        public string EmployerAddress { get; set; }
        public string Designation { get; set; }
        public string Sector { get; set; }
        public Branch Branch { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactAddress { get; set; }
        public string NextOfKin { get; set; }
        public string NokAddress { get; set; }
        public string NoKPhoneNumber { get; set; }
        public string NokRelationship { get; set; }

        //For corporate customers
        public string IncorporationType { get; set; }
        public string RegisteredBody { get; set; }
        public string RCNo { get; set; }
        public int CustomerBase { get; set; }
        public decimal StartupCapital { get; set; }
        public decimal Turnover { get; set; }
        public string Website { get; set; }

        public string ProfileImage { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime TransDate { get; set; }
        public string Status { get; set; }

        //Navigation 
        public ICollection<Account> Accounts { get; set; }
    }
}
