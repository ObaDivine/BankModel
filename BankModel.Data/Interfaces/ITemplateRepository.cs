using System.Collections.Generic;
using System.Threading.Tasks;
using BankModel.Models;
using BankModel.Models.ViewModels;

namespace BankModel.Data.Interfaces
{
    public interface ITemplateRepository
    {
        //Account template details
        #region
        bool ProductCodeExist(string productCode);
        bool AccountTemplateExist(string templateName);
        IEnumerable<TemplateAccount> GetAccountTemplate(string username);
        TemplateAccountViewModel GetAccountTemplateByID(int ID);
        Task<string> CreateAccountTemplateAsync(TemplateAccountViewModel model);
        Task<string> UpdateAccountTemplateAsync(TemplateAccountViewModel model);
        Task<string> DropAccountTemplateAsync(int id);
        bool IsAccountTemplateInUse(int id);
        TemplateAccount GetAccountTemplateDetails(int id);
        #endregion
    }
}
