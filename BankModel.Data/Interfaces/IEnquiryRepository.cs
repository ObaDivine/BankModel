using BankModel.Models;
using BankModel.Models.ViewModels;
using System.Collections;
using System.Collections.Generic;

namespace BankModel.Data.Interfaces
{
    public interface IEnquiryRepository
    {
        IEnumerable<Account> GetCustomerAccounts(string accountNo);
        IEnumerable GetAccountTransactions(string accountNo);
        EnquiryViewModel GetCustomerAccountDetails(string accountNo);
        Profile GetCustomerProfile(string ID);
    }
}
