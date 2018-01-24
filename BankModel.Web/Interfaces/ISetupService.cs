using BankModel.Models;
using BankModel.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Web.Interfaces
{
    public interface ISetupService
    {
        //Branch details
        Task<bool> IsBranchInUse(int id);
        Task<IEnumerable<string>> GetBranchNames(string username);
        Task<IEnumerable<Branch>> GetBranchesWithDetails();
        Task<BranchViewModel> GetBranchWithDetails(int id);
        Task<bool> CreateBranchAsync(BranchViewModel model);
        Task<bool> UpdateBranchAsync(BranchViewModel model);
        Task<bool> DropBranchAsync(int id);

        //Account sub head details
        Task<bool> IsAccountSubHeadInUse(int id);
        Task<IEnumerable<ChartOfAccountSubHead>> GetAccountSubHeads();
        Task<AccountSubHeadViewModel> GetAccountSubHeads(int id);
        Task<bool> CreateAccountSubHeadAsync(AccountSubHeadViewModel model);
        bool UpdateAccountSubHeadAsync(AccountSubHeadViewModel model);
        Task<bool> DropAccountSubHeadAsync(int id);
    }
}
