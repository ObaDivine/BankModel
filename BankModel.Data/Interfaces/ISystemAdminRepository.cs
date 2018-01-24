using BankModel.Models;
using BankModel.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Data.Interfaces
{
    public interface ISystemAdminRepository
    {
        Task<string> CreateSystemUserAsync(SystemUsersViewModel model);
        List<Parameter> GetApplicationParameters();
        IEnumerable<SystemUserDetailsViewModel> GetSystemUsers();
        IEnumerable<string> GetBranchStaff(string branch);
        Task<SystemUserDetailsViewModel> GetUserDetails(string username);
        SystemUsersViewModel GetSystemUserWithDetails(string ID);
        bool IsSystemUserInUse(string id);
        Task<string> UpdateSystemUserAsync(SystemUsersViewModel model);
        Task<string> DropSystemUserAsync(string id);
        string MaintainSystemUser(SystemUsersViewModel model);
    }
}
