using BankModel.Models.ViewModels;
using System.Threading.Tasks;

namespace BankModel.Data.Interfaces
{
    public interface IRoutineProcessingRepository
    {
        bool NoEODPendingItems();
        int GetPendingSystemUsers();
        int GetPendingBranch();
        int GetPendingChartofAccount();
        int GetPendingProfile();
        int GetPendingAccount();
        int GetPendingTransaction();
        int GetPendingLoan();
        int GetPendingLoanRepayment();
        int GetPendingFixedDeposit();
        int GetPendingSalary();
        int GetPendingMobileMoney();
        string GetTransactionDate();
        bool IsEOMLastDay();
        bool EOMLastDayCompleted();
        bool EOMCompleted();
        Task<string> EOD(EODViewModel model);
        string EOMLastDaySavingsInterest(string username);
        string EOMLastDayOverdrawn();
        Task<string> EOMSavingsInterest();
        Task<string> EOMOverdrawnAccount();
        Task<string> EOMSMS();
        Task<string> EOMLoanRepayment();
        Task<string> EOMLoanDefault();
        Task<string> EOMFixedDeposit();
        Task<string> EOMStandingOrder();
        Task<string> EOMProfitandLoss();
        Task<string> EOMBalanceSheet();
        Task<string> StartofMonth(EODViewModel model);
        Task<string> EOY(EODViewModel model);
    }
}
