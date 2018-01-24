using System.ComponentModel.DataAnnotations;

namespace BankModel.Models.ViewModels
{
    public class SystemUsersViewModel
    {
        [DataType(DataType.Text)]
        public string Staff { get; set; }

        [DataType(DataType.Text)]
        public string Username { get; set; }

        [DataType(DataType.Text)]
        public string Branch { get; set; }

        [DataType(DataType.Text)]
        public decimal ApprovalLimit { get; set; }

        [DataType(DataType.Text)]
        public decimal TransactionLimit { get; set; }

        [DataType(DataType.Text)]
        public string Role { get; set; }

        public string ID { get; set; }

        public bool ActivateUser { get; set; }

        public string MaintenanceType { get; set; }

        public int PasswordDays { get; set; }

        public decimal LimitField { get; set; }

        [DataType(DataType.Text)]
        public string NewBranch { get; set; }

        public string StatusMessage { get; set; }

        public string ActionBy { get; set; }
    }
}
