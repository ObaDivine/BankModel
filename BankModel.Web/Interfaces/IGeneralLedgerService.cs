using BankModel.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Web.Interfaces
{
    public interface IGeneralLedgerService
    {
        //Chart of account details
        Task<bool> IsChartofAccountInUse(long id);
        Task<bool> CreateChartofAccountAsync(ChartofAccountViewModel model);
        Task<bool> UpdateChartofAccountAsync(ChartofAccountViewModel model);
        Task<bool> DropChartofAccountAsync(int id);
        Task<IEnumerable<string>> GetAccountSubHeads(string accountHead);
        Task<IEnumerable<BranchChartofAccountViewModel>> GetChartofAccountByUser(string username);
        Task<IEnumerable<string>> GetBranchNamesByUser(string username);
        Task<ChartofAccountViewModel> GetChartofAccount(long id);
    }
}
