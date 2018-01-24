using System.ComponentModel.DataAnnotations;


namespace BankModel.Models.ViewModels
{
    public class ChartofAccountViewModel
    {

        [DataType(DataType.Text)]
        public string AccountName { get; set; }

        [DataType(DataType.Text)]
        public string AccountHead { get; set; }

        [DataType(DataType.Text)]
        public string AccountSubHead { get; set; }

        [DataType(DataType.Text)]
        public string Branch { get; set; }

        public long ID { get; set; }
        public string StatusMessage { get; set; }
        public string ActionBy { get; set; }
    }
}