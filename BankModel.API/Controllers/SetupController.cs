using BankModel.Data.Interfaces;
using BankModel.Models.ViewModels;
using BankModel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace BankModel.API.Controllers
{
    [Route("api/[controller]")]
    public class SetupController : Controller
    {
        private readonly ISetupRepository _setupRepository;

        public SetupController(ISetupRepository setupRepository)
        {
            _setupRepository = setupRepository;
        }

        #region
        [Route("api.bankmodel/[controller]/brancheswithdetails")]
        [HttpGet]
        public IEnumerable<Branch> GetBranchesWithDetails()
        {
            return _setupRepository.GetBranchesWithDetails();
        }

        [Route("api.bankmodel/[controller]/brancheswithdetails")]
        [HttpGet("{id}")]
        public BranchViewModel GetBranchWithDetails(int id)
        {
            return _setupRepository.GetBranchWithDetails(id);
        }

        [Route("api.bankmodel/[controller]/branches")]
        [HttpGet("{id}")]
        public IEnumerable<string> GetBranches(string id)
        {
            return _setupRepository.GetBranchNames(id);
        }

        [Route("api.bankmodel/[controller]/branchcode")]
        [HttpGet("{id}")]
        public bool CheckIfBranchCodeExist(string id)
        {
            return _setupRepository.CheckIfBranchCodeExist(id);
        }

        [Route("api.bankmodel/[controller]/branchdescription")]
        [HttpGet("{id}")]
        public bool CheckIfBranchDescriptionExist(string id)
        {
            return _setupRepository.CheckIfBranchDescriptionExist(id);
        }

        [Route("api.bankmodel/setup/branchinuse")]
        [HttpGet("{id}")]
        public bool IsBranchInUse(int id)
        {
            return _setupRepository.IsBranchInUse(id);
        }

        [Route("api.bankmodel/setup/createbranch")]
        [HttpPost]
        public async Task<IActionResult> CreateBranch(BranchViewModel model)
        {
            var result = await _setupRepository.CreateBranchAsync(model);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api.bankmodel/setup/updatebranch")]
        [HttpPut]
        public async Task<IActionResult> UpdateBranch(BranchViewModel model)
        {
            var result = await _setupRepository.UpdateBranchAsync(model);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api.bankmodel/setup/dropbranch")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DropBranch(int id)
        {
            var result = await _setupRepository.DropBranchAsync(id);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }
        #endregion

        #region
        [Route("api.bankmodel/[controller]/accountsubheads")]
        [HttpGet]
        public IEnumerable<ChartOfAccountSubHead> GetAccountSubHeads()
        {
            return _setupRepository.GetAccountSubHeads();
        }

        [Route("api.bankmodel/[controller]/accountsubhead")]
        [HttpGet("{id}")]
        public AccountSubHeadViewModel GetAccountSubHeads(int id)
        {
            return _setupRepository.GetAccountSubHeads(id);
        }

        [Route("api.bankmodel/setup/accountsubheadinuse")]
        [HttpGet("{id}")]
        public bool IsAccountSubHeadInUse(int id)
        {
            return _setupRepository.IsAccountSubHeadInUse(id);
        }

        [Route("api.bankmodel/setup/accountsubheadcode/{accountCode}/{accountHead}")]
        [HttpGet]
        public bool CheckIfAccountSubHeadCodeExist(string accountCode, string accountHead)
        {
            var result = _setupRepository.AccountSubHeadCodeExist(accountCode, accountHead);
            return result;
        }

        [Route("api.bankmodel/setup/accountsubheadname")]
        [HttpPost]
        public bool CheckIfAccountSubHeadNameExist(AccountSubHeadViewModel model)
        {
            var result = _setupRepository.AccountSubHeadNameExist(model.AccountName, model.AccountHead);
            return result;
        }

        [Route("api.bankmodel/setup/createaccountsubhead")]
        [HttpPost]
        public async Task<IActionResult> CreateAccountSubHead(AccountSubHeadViewModel model)
        {
            var result = await _setupRepository.CreateAccountSubHeadAsync(model);
            if(result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api.bankmodel/setup/updateaccountsubhead")]
        [HttpPut]
        public async Task<IActionResult> UpdateAccountSubHead(AccountSubHeadViewModel model)
        {
            var result = await _setupRepository.UpdateAccountSubHeadAsync(model);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api.bankmodel/setup/deleteaccountsubhead")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DropAccountSubHead(int id)
        {
            var result = await _setupRepository.DropAccountSubHeadAsync(id);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }
        #endregion
    }
}
