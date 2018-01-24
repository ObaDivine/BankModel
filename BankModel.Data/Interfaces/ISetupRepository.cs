using BankModel.Models;
using BankModel.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Data.Interfaces
{
    public interface ISetupRepository
    {
        //Branch details
        IEnumerable<string> GetBranchNames(string username);
        IEnumerable<Branch> GetBranchesWithDetails();
        BranchViewModel GetBranchWithDetails(int ID);
        bool CheckIfBranchCodeExist(string branchCode);
        bool CheckIfBranchDescriptionExist(string branchDescription);
        Task<string> CreateBranchAsync(BranchViewModel model);
        Task<string> UpdateBranchAsync(BranchViewModel model);
        Task<string> DropBranchAsync(int id);
        bool IsBranchInUse(int id);

        //Account sub head details
        bool IsAccountSubHeadInUse(int id);
        IEnumerable<ChartOfAccountSubHead> GetAccountSubHeads();
        IEnumerable<string> GetAccountSubHeads(string accountHead);
        AccountSubHeadViewModel GetAccountSubHeads(int id);
        bool AccountSubHeadCodeExist(string accountCode, string accountHead);
        bool AccountSubHeadNameExist(string accountName, string accountHead);
        Task<string> CreateAccountSubHeadAsync(AccountSubHeadViewModel model);
        Task<string> UpdateAccountSubHeadAsync(AccountSubHeadViewModel model);
        Task<string> DropAccountSubHeadAsync(int id);
    }
}
