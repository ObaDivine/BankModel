using BankModel.Models;
using BankModel.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BankModel.Web.Interfaces
{
    public interface ITemplateService
    {
        //Handle account template processes
        #region
        Task<bool> IsAccountTemplateInUse(int id);
        Task<IEnumerable<TemplateAccount>> GetAccountTemplateByUser(string username);
        Task<TemplateAccount> GetAccountTemplateDetails(int id);
        Task<TemplateAccountViewModel> GetAccountTemplateByID(int id);
        Task<bool> HasApprovalRole(string username);
        Task<bool> CreateAccountTemplateAsync(TemplateAccountViewModel model);
        Task<bool> UpdateAccountTemplateAsync(TemplateAccountViewModel model);
        Task<bool> DropAccountTemplateAsync(int id);
        #endregion
    }
}
