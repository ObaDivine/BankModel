namespace BankModel.Models.ViewModels
{
    public class BranchChartofAccountViewModel
    {
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string AccountHead { get; set; }
        public string AccountSubHead { get; set; }
        public string Branch { get; set; }
        public string Status { get; set; }

        public long ID { get; set; }
        public string StatusMessage { get; set; }
    }
}