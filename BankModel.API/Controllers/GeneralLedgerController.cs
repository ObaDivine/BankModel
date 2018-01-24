using BankModel.Data.Interfaces;
using BankModel.Models.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BankModel.API.Controllers
{
    [Route("api/[controller]")]
    public class GeneralLedgerController : Controller
    {
        private readonly IGeneralLedgerRepository _glRepository;

        public GeneralLedgerController(IGeneralLedgerRepository glRepository)
        {
            _glRepository = glRepository;
        }

        [Route("api.bankmodel/[controller]/gl-account-inuse")]
        [HttpGet("{id}")]
        public IActionResult IsChartofAccountInUse(long id)
        {
            return Ok(_glRepository.IsChartofAccountInUse(id));
        }

        [Route("api.bankmodel/[controller]/gl-account-exist")]
        [HttpGet]
        public IActionResult ChartofAccountExist(ChartofAccountViewModel model)
        {
            return Ok(_glRepository.ChartofAccountExist(model));
        }

        [Route("api.bankmodel/[controller]/username-exist")]
        [HttpGet("{id}")]
        public IActionResult UsernameExist(string id)
        {
            return Ok(_glRepository.UsernameExist(id));
        }

        [Route("api.bankmodel/[controller]/create-gl-account")]
        [HttpGet("{id}")]
        public async Task<IActionResult> CreateChartOfAccount(ChartofAccountViewModel model)
        {
            var result = await _glRepository.CreateChartofAccountAsync(model);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api.bankmodel/[controller]/drop-gl-account")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DropChartOfAccount(int id)
        {
            var result = await _glRepository.DropChartofAccountAsync(id);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api.bankmodel/[controller]/account-subheads")]
        [HttpGet("{id}")]
        public IActionResult GetChartofAccountSubHeads(string id)
        {
            var result =  _glRepository.GetAccountSubHeads(id);
            if (result == null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [Route("api.bankmodel/[controller]/gl-account-by-user")]
        [HttpGet("{id}")]
        public IActionResult GetChartofAccountByUser(string id)
        {
            var result = _glRepository.GetChartofAccountByUser(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [Route("api.bankmodel/[controller]/branches-by-user")]
        [HttpGet("{id}")]
        public IActionResult GetBranchNamesByUser(string id)
        {
            var result = _glRepository.GetBranchNamesByUser(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [Route("api.bankmodel/[controller]/gl-account-by-id")]
        [HttpGet("{id}")]
        public IActionResult GetChartofAccount(long id)
        {
            var result = _glRepository.GetChartofAccount(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

    }
}
