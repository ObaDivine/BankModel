using BankModel.Data.Interfaces;
using BankModel.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankModel.API.Controllers
{
    [Route("api/[controller]")]
    public class CustomerServiceController : Controller
    {
        private readonly ICustomerServiceRepository _customerServiceRepository;

        public CustomerServiceController(ICustomerServiceRepository customerServiceRepository)
        {
            _customerServiceRepository = customerServiceRepository;
        }

        #region
        [Route("api.bankmodel/[controller]/profile-inuse")]
        [HttpGet("{id}")]
        public IActionResult IsProfileInUse(string id)
        {
            return Ok(_customerServiceRepository.IsProfileInUse(id));
        }

        [Route("api.bankmodel/[controller]/is-admin")]
        [HttpGet("{username}")]
        public IActionResult IsAdministrator(string username)
        {
            return Ok(_customerServiceRepository.IsAdministrator(username));
        }

        [Route("api.bankmodel/[controller]/individual-profile-exist")]
        [HttpGet]
        public IActionResult CheckIfProfileExist(IndividualProfileViewModel model)
        {
            return Ok(_customerServiceRepository.CheckIfProfileExist(model.Lastname, model.Othernames, model.DateOfBirth));
        }

        [Route("api.bankmodel/[controller]/phone-number-exist")]
        [HttpGet("{phoneNumber}")]
        public IActionResult CheckIfPhoneNumberExist(string phoneNumber)
        {
            return Ok(_customerServiceRepository.CheckIfPhoneNumberExist(phoneNumber));
        }

        [Route("api.bankmodel/[controller]/email-exist")]
        [HttpGet("{email}")]
        public IActionResult CheckIfEmailExist(string email)
        {
            return Ok(_customerServiceRepository.CheckIfEmailExist(email));
        }

        [Route("api.bankmodel/[controller]/create-individual-profile")]
        [HttpPost]
        public async Task<IActionResult> CreateIndividualProfile(IndividualProfileViewModel model)
        {
            var result = await _customerServiceRepository.CreateIndividualProfileAsync(model);
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/update-individual-profile")]
        [HttpPut]
        public async Task<IActionResult> UpdateIndividualProfile(IndividualProfileViewModel model)
        {
            var result = await _customerServiceRepository.UpdateIndividualProfileAsync(model);
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/drop-profile")]
        [HttpDelete("id")]
        public async Task<IActionResult> DropProfile(string id)
        {
            var result = await _customerServiceRepository.DropProfileAsync(id);
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/generated-customer-number")]
        [HttpGet]
        public IActionResult GetGeneratedCustomerNo()
        {
            var result = _customerServiceRepository.GetGeneratedCustomerNo();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        #endregion

        #region
        [Route("api.bankmodel/[controller]/corporate-profile-exist")]
        [HttpGet]
        public IActionResult CheckIfCorporateProfileExist(CorporateProfileViewModel model)
        {
            return Ok(_customerServiceRepository.CheckIfProfileExist(model.CustomerName, model.DateOfIncorporation));
        }

        [Route("api.bankmodel/[controller]/update-corporate-profile")]
        [HttpPut]
        public async Task<IActionResult> UpdateCorporateProfile(CorporateProfileViewModel model)
        {
            var result = await _customerServiceRepository.UpdateCorporateProfileAsync(model);
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/get-corporate-profile")]
        [HttpGet]
        public CorporateProfileViewModel GetCorporateProfile(string id, int customer = 0)
        {
            return _customerServiceRepository.GetCorporateProfile(id, customer);
        }

        #endregion

        //Handle account transactions
        #region
        [Route("api.bankmodel/[controller]/account-inuse")]
        [HttpGet("{accountNo}")]
        public IActionResult IsAccountInUse(string accountNo)
        {
            return Ok(_customerServiceRepository.IsAccountInUse(accountNo));
        }

        [Route("api.bankmodel/[controller]/account-exist")]
        [HttpGet("{accountNo}")]
        public IActionResult CheckIfCustomerAccountExist(CustomerAccountViewModel model)
        {
            return Ok(_customerServiceRepository.CheckIfCustomerAccountExist(model.Customer, model.Product));
        }

        [Route("api.bankmodel/[controller]/create-customer-account")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAccount(CustomerAccountViewModel model)
        {
            var result = await _customerServiceRepository.CreateCustomerAccountAsync(model);
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/update-customer-account")]
        [HttpPut]
        public async Task<IActionResult> UpdateCustomerAccount(CustomerAccountViewModel model)
        {
            var result = await _customerServiceRepository.UpdateCustomerAccountAsync(model);
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/drop-customer-account")]
        [HttpDelete("id")]
        public async Task<IActionResult> DropCustomerAccount(string id)
        {
            var result = await _customerServiceRepository.DropCustomerAccountAsync(id);
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/generated-account-number")]
        [HttpGet]
        public IActionResult GetGeneratedAccountNo()
        {
            var result = _customerServiceRepository.GetGeneratedAccountNo();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        #endregion
    }
}
