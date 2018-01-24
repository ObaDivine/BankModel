using System;
using System.Collections.Generic;
using BankModel.Web.ViewModels;


namespace BankModel.Web.Interfaces
{
    public interface IAccountService
    {
        DateTime GetPasswordExpiryDate(string username);
        string GetUserStatus(string username);
        string GetProfileImage(string username);
        string GetUserRole(string username);
        string GetUserBranch(string username);
        string GetTransactionDate();
        List<string> ValidateLoginRequirement(LoginViewModel model);
        void SetUserLogin(string username);
        void ClearUserLogin(string username);
    }
}
