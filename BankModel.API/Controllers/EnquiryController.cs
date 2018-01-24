using BankModel.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace BankModel.API.Controllers
{
    [Route("api/[controller]")]
    public class EnquiryController : Controller
    {
        private readonly IEnquiryRepository _enquiryRepository;

        public EnquiryController(IEnquiryRepository enquiryRepository)
        {
            _enquiryRepository = enquiryRepository;
        }

        [Route("api.bankmodel/[controller]/customer-account")]
        [HttpGet("{id}")]
        public IActionResult GetCustomerAccounts(string id)
        {
            var result = _enquiryRepository.GetCustomerAccounts(id);
            if(result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [Route("api.bankmodel/[controller]/customer-account-details")]
        [HttpGet("{id}")]
        public IActionResult GetCustomerAccountDetails(string id)
        {
            var result = _enquiryRepository.GetCustomerAccountDetails(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [Route("api.bankmodel/[controller]/customer-profile")]
        [HttpGet("{id}")]
        public IActionResult GetCustomerProfile(string id)
        {
            var result = _enquiryRepository.GetCustomerProfile(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [Route("api.bankmodel/[controller]/account-transactions")]
        [HttpGet("{id}")]
        public IActionResult GetAccountTransactions(string id)
        {
            var result = _enquiryRepository.GetAccountTransactions(id);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
    }
}
