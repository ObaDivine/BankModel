using System;

namespace BankModel.Data.Interfaces
{
    public interface IAccountRepository
    {
        DateTime GetPasswordExpiryDate(string username);
        string GetUserStatus(string username);
        string GetProfileImage(string username);
        string GetUserRole(string username);
        string GetUserBranch(string username);
        string GetTransactionDate();
        bool UserIsLoggedIn(string username);
        void SetUserLogin(string username);
        void ClearUserLogin(string username);
    }
}
