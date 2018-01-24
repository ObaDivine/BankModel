using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankModel.Models
{
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string BranchCode { get; set; }
        public string BranchDesc { get; set; }
        public string BranchLocation { get; set; }
        public string BranchManager { get; set; }
        public int ChartofAccountCounter { get; set; }
        public DateTime TransDate { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Status { get; set; }

        //Navigation
        public ICollection<Profile> Profile { get; set; }
        public ICollection<ChartOfAccount> ChartOfAccount { get; set; }
    }
}
