using BankModel.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Web.Interfaces
{
    public interface ICustomerService
    {
        //Handle individual profile processes
        #region
        Task<bool> CreateIndividualProfileAsync(IndividualProfileViewModel model);
        Task<bool> UpdateIndividualProfileAsync(IndividualProfileViewModel model);
        Task<bool> DropProfileAsync(string id);
        Task<bool> IsProfileInUse(string id);
        Task<bool> IsAdministrator(string username);
        Task<string> GetIndividualProfilePrefix();
        Task<string> GetGeneratedCustomerNo();
        #endregion

        //Handle corporate profile processes
        #region
        Task<bool> CreateCorporateProfileAsync(CorporateProfileViewModel model);
        Task<bool> UpdateCorporateProfileAsync(CorporateProfileViewModel model);
        Task<CorporateProfileViewModel> GetCorporateProfile(string id, int customer = 0);
        #endregion

        //Handle customer accounts 
        #region
        Task<bool> CreateCustomerAccountAsync(CustomerAccountViewModel model);
        Task<bool> UpdateCustomerAccountAsync(CustomerAccountViewModel model);
        Task<bool> DropCustomerAccountAsync(string id);
        Task<bool> IsAccountInUse(string id);
        Task<string> GetGeneratedAccountNo();
        #endregion
    }
}
