using BankModel.Models;
using BankModel.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Web.Interfaces
{
    public interface ISystemAdminService
    {
        Task<bool> CreateSystemUserAsync(SystemUsersViewModel model);
        Task<bool> UpdateSystemUserAsync(SystemUsersViewModel model);
        Task<bool> DropSystemUserAsync(string id);
        bool MaintainSystemUser(SystemUsersViewModel model);
        Task<IEnumerable<string>> GetBranchStaff(string branch);
        Task<IEnumerable<SystemUserDetailsViewModel>> GetSystemUsers();
        Task<SystemUserDetailsViewModel> GetUserDetails(string user);
        Task<SystemUsersViewModel> GetSystemUserWithDetails(string user);
        Task<List<Parameter>> GetApplicationParameters();
    }
}
