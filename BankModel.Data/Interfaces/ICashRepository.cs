using BankModel.Web;
using BankModel.Web.ViewModels;
using System.Collections;
using System.Threading.Tasks;

namespace BankModel.Data.Interfaces
{
    public interface ICashRepository
    {
        string GetBranchCodeFromAccountNo(string accountNo);
        bool InterBranchAccountExist(string debitAccount, string creditAccount);
        string GetProductCodeFromAccountNo(string accountNo);
        string GetLiabilityAccountNo(string productCode, string branchCode);

        //Deposit transactions
        #region
        bool CheckCashDepositTransactionLimitReached(string accountNo);
        bool CheckCashDepositAmountLimitReached(string accountNo);
        Task<string> CreateCashDepositAsync(TransactionViewModel model, string username);
        IEnumerable GetCashDeposit(string username);
        #endregion


        //Withdrawal transactions
        #region
        bool CheckCashWithdrawalTransactionLimitReached(string accountNo);
        bool CheckCashWithdrawalAmountLimitReached(string accountNo);
        Task<string> CreateCashWithdrawalAsync(TransactionViewModel model, string username);
        IEnumerable GetCashWithdrawal(string username);
        bool AllowOverdraw(string accountNo);
        #endregion


        //Fund transfer transactions
        #region
        Task<string> CreateFundTransferAsync(TransactionViewModel model, string username);
        IEnumerable GetFundTransfer(string username);
        #endregion

        Account CheckIfAccountExist(string accountNo);
        string GetTellerAccount(string username);
        string CheckAccountStatus(string accountNo);
        string[] GetAccountMandate(string accountNo);
        bool CheckIfTeller(string username);
        bool CheckPostNoCredit(string accountNo);
        bool CheckPostNoDebit(string accountNo);
        bool TransactionLimitReached(string username, decimal amount);
        bool CheckIfAccountIsFunded(string transType, string accountNo, decimal amount);
        Task<bool> CheckIfUserHasRole(string username);
    }
}
