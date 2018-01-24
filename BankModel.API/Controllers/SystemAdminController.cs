using BankModel.Data.Interfaces;
using BankModel.Models;
using BankModel.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.API.Controllers
{
    [Route("api/[controller]")]
    public class SystemAdminController : Controller
    {
        private readonly ISystemAdminRepository _systemAdminRepository;
        public SystemAdminController(ISystemAdminRepository systemAdminRepository)
        {
            _systemAdminRepository = systemAdminRepository;
        }

        [Route("api.bankmodel/[controller]/branchstaff")]
        [HttpGet("{id}")]
        public IEnumerable<string> GetBranchStaff(string id)
        {
            return _systemAdminRepository.GetBranchStaff(id);
        }

        [Route("api.bankmodel/[controller]/systemuserinuse")]
        [HttpGet("{id}")]
        public bool IsSystemUserInUse(string id)
        {
            return _systemAdminRepository.IsSystemUserInUse(id);
        }

        [Route("api.bankmodel/[controller]/systemusers")]
        [HttpGet]
        public IEnumerable<SystemUserDetailsViewModel> GetSystemUsers()
        {
            return _systemAdminRepository.GetSystemUsers();
        }

        [Route("api.bankmodel/[controller]/systemuserdetails")]
        [HttpGet("{id}")]
        public SystemUsersViewModel GetSystemUserWithDetails(string id)
        {
            return _systemAdminRepository.GetSystemUserWithDetails(id);
        }

        [Route("api.bankmodel/[controller]/userdetails")]
        [HttpGet("{id}")]
        public async Task<SystemUserDetailsViewModel> GetUserDetails(string id)
        {
            var result = await _systemAdminRepository.GetUserDetails(id);
            return result;
        }

        [Route("api.bankmodel/systemadmin/applicationparameters")]
        [HttpGet]
        public List<Parameter> GetApplicationParameters()
        {
            return _systemAdminRepository.GetApplicationParameters();
        }

        [Route("api.bankmodel/systemadmin/createsystemuser")]
        [HttpPost]
        public async Task<IActionResult> CreateSystemUser(SystemUsersViewModel model)
        {
            var result = await _systemAdminRepository.CreateSystemUserAsync(model);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api.bankmodel/systemadmin/updatesystemuser")]
        [HttpPut]
        public async Task<IActionResult> UpdateSystemUser(SystemUsersViewModel model)
        {
            var result = await _systemAdminRepository.UpdateSystemUserAsync(model);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api.bankmodel/systemadmin/dropsystemuser")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DropSystemUser(string id)
        {
            var result = await _systemAdminRepository.DropSystemUserAsync(id);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}
