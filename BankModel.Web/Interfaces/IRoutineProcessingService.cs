using BankModel.Web.ViewModels;
using System.Threading.Tasks;

namespace BankModel.Web.Interfaces
{
    public interface IRoutineProcessingService
    {
        //End of day details
        Task<bool> NoEODPendingItems();
        Task<int> GetPendingSystemUsers();
        Task<int> GetPendingBranch();
        Task<int> GetPendingChartofAccount();
        Task<int> GetPendingProfile();
        Task<int> GetPendingAccount();
        Task<int> GetPendingTransaction();
        Task<int> GetPendingLoan();
        Task<int> GetPendingLoanRepayment();
        Task<int> GetPendingFixedDeposit();
        Task<int> GetPendingSalary();
        Task<int> GetPendingMobileMoney();
        string EOD(EODViewModel model);

        //End of month details
        Task<bool> IsEOMLastDay();
        Task<bool> EOMLastDayCompleted();
        Task<bool> EOMCompleted();
        Task<string> EOMLastDaySavingsInterest(string username);
        Task<string> EOMLastDayOverdrawn();
        Task<string> EOMSavingsInterest();
        Task<string> EOMOverdrawnAccount();
        Task<string> EOMSMS();
        Task<string> EOMLoanRepayment();
        Task<string> EOMLoanDefault();
        Task<string> EOMFixedDeposit();
        //Task<string> EOMOverdrawn();
        Task<string> EOMStandingOrder();
        Task<string> EOMProfitandLoss();
        Task<string> EOMBalanceSheet();
        Task<string> StartofMonth(EODViewModel model);

        //End of year detai;s
        Task<string> EOY(EODViewModel model);
    }
}
