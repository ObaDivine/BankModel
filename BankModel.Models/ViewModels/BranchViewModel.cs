using System.ComponentModel.DataAnnotations;


namespace BankModel.Models.ViewModels
{
    public class BranchViewModel
    {

        [DataType(DataType.Text)]
        public string BranchCode { get; set; }

        [DataType(DataType.Text)]
        public string BranchDesc { get; set; }

        [DataType(DataType.Text)]
        public string BranchLocation { get; set; }

        [DataType(DataType.Text)]
        public string BranchManager { get; set; }

        public int ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }
    }
}