using BankModel.Web;
using BankModel.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Data.Interfaces
{
    public interface IApprovalRepository
    {
        //System users
        #region
        IEnumerable<SystemUserDetailsViewModel> GetPendingSystemUsers(string postedBy);
        Task<string> ApproveSystemUsersAsync(string id, string username);
        #endregion

        //Branch details
        #region
        IEnumerable<Branch> GetPendingBranches(string username);
        Task<string> ApproveBranchesAsync(int id, string username);
        #endregion

        //Chart of account
        #region
        IEnumerable<BranchChartofAccountViewModel> GetPendingChartofAccount(string username);
        Task<string> ApproveChartofAccountAsync(long id, string username);
        #endregion

        //Account Template
        #region
        IEnumerable<TemplateAccount> GetPendingAccountTemplate(string username);
        Task<string> ApproveAccountTemplateAsync(int id, string username);
        TemplateAccount GetAccountTemplateDetails(int id);
        #endregion

        //Profiles 
        #region
        IEnumerable<Profile> GetPendingProfile(string username);
        Task<string> ApproveProfileAsync(string id, string username);
        #endregion
       
        //Customer account
        #region
        IEnumerable<Account> GetPendingCustomerAccount(string username);
        Task<string> ApproveCustomerAccountAsync(string id, string username);
        #endregion
    }
}
