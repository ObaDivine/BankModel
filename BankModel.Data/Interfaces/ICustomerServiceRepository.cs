using BankModel.Models;
using BankModel.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Data.Interfaces
{
    public interface ICustomerServiceRepository
    {
        IEnumerable<string> GetLGAs(string state);
        IEnumerable<string> GetStates();
        bool IsProfileInUse(string id);
        bool CheckIfPhoneNumberExist(string phoneNumber);
        bool CheckIfEmailExist(string email);
        Task<bool> IsAdministrator(string username);
        string GetGeneratedAccountNo();
        Profile GetProfileDetails(string id);
        IEnumerable<string> GetBranchNames();

        //Handle customer accounts
        #region
        Account GetAccountDetails(string id);
        IEnumerable<string> GetCustomers(string customerType, string username);
        IEnumerable<string> GetAccountTemplateNames();
        IEnumerable<string> GetBranchStaff(string username);
        string GetGeneratedCustomerNo();
        Account CheckIfCustomerAccountExist(string customerName, string product);
        bool IsAccountInUse(string id);
        Task<string> CreateCustomerAccountAsync(CustomerAccountViewModel model);
        Task<string> UpdateCustomerAccountAsync(CustomerAccountViewModel model);
        Task<string> DropCustomerAccountAsync(string id);
        IEnumerable<string> GetSystemUsers(string username);
        IEnumerable<Account> GetCustomerAccount(string username);
        CustomerAccountViewModel GetCustomerAccount(string id, int customer = 0);
        #endregion

        //Handle individual profile
        #region
        bool CheckIfProfileExist(string lastname, string othername, DateTime dateofBirth);
        Task<string> CreateIndividualProfileAsync(IndividualProfileViewModel model);
        Task<string> UpdateIndividualProfileAsync(IndividualProfileViewModel model);
        Task<string> DropProfileAsync(string id);
        IEnumerable<Profile> GetIndividualProfile(string username);
        IndividualProfileViewModel GetIndividualProfile(string id, int customer = 0);
        string GetIndividualProfilePrefix();
        #endregion

        //Handle corporate profile
        #region
        Profile CheckIfProfileExist(string businessName, DateTime dateofIncorporation);
        Task<string> CreateCorporateProfileAsync(CorporateProfileViewModel model);
        Task<string> UpdateCorporateProfileAsync(CorporateProfileViewModel model);
        IEnumerable<Profile> GetCorporateProfile(string username);
        CorporateProfileViewModel GetCorporateProfile(string id, int customer = 0);
        #endregion
    }
}
