using BankModel.Models;
using BankModel.Web.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Web.Interfaces
{
    public interface IEnquiryService
    {
        Task<IEnumerable<Account>> GetCustomerAccounts(string accountNo);
        Task<EnquiryViewModel> GetCustomerAccountDetails(string accountNo);
        Task<Profile> GetCustomerProfile(string accountNo);
        Task<IEnumerable> GetAccountTransactions(string accountNo);
    }
}
