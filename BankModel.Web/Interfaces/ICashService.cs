using BankModel.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BankModel.Web.Interfaces
{
    public interface ICashService
    {
        string GetTellerAccount(string username);
        //Deposit transaction
        #region
        Task<List<string>> ValidateCashDepositRequirement(TransactionViewModel model, string username);
        Task<string> CreateCashDepositAsync(TransactionViewModel model, string username);
        #endregion

        //Withdrawal transaction
        #region
        Task<List<string>> ValidateCashWithdrawalRequirement(TransactionViewModel model, string username);
        Task<string> CreateCashWithdrawalAsync(TransactionViewModel model, string username);
        #endregion

        //Fund transfer transaction
        #region
        Task<List<string>> ValidateFundTransferRequirement(TransactionViewModel model, string username);
        Task<string> CreateFundTransferAsync(TransactionViewModel model, string username);
        #endregion
    }
}
