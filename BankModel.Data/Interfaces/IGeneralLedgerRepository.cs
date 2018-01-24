using BankModel.Models;
using BankModel.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Data.Interfaces
{
    public interface IGeneralLedgerRepository
    {
        //Handle chart of account processes
        #region 
        IEnumerable<string> GetBranchNamesByUser(string username);
        IEnumerable<string> GetAccountSubHeads(string accountHead);
        bool IsChartofAccountInUse(long id);
        IEnumerable<BranchChartofAccountViewModel> GetChartofAccountByUser(string username);
        ChartofAccountViewModel GetChartofAccount(long id);
        bool ChartofAccountExist(ChartofAccountViewModel model);
        string GetBranchCodeFromName(string branchName);
        Task<string> CreateChartofAccountAsync(ChartofAccountViewModel model);
        Task<string> UpdateChartofAccountAsync(ChartofAccountViewModel model);
        Task<string> DropChartofAccountAsync(int id);
        bool UsernameExist(string username);
        #endregion

        //Handle gl transactions and posting
        #region

        #endregion
    }
}
