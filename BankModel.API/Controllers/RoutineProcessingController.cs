using BankModel.Data.Interfaces;
using BankModel.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankModel.API.Controllers
{
    [Route("api/[controller]")]
    public class RoutineProcessingController : Controller
    {
        private readonly IRoutineProcessingRepository _rpRepository;

        public RoutineProcessingController(IRoutineProcessingRepository rpRepository)
        {
            _rpRepository = rpRepository;
        }


        //Handle EOD pending transactions count
        #region
        [Route("api.bankmodel/[controller]/pending-users")]
        [HttpGet]
        public IActionResult GetPendingSystemUsers()
        {
            return Ok(_rpRepository.GetPendingSystemUsers());
        }

        [Route("api.bankmodel/[controller]/pending-branch")]
        [HttpGet]
        public IActionResult GetPendingBranch()
        {
            return Ok(_rpRepository.GetPendingBranch());
        }

        [Route("api.bankmodel/[controller]/pending-chart-of-account")]
        [HttpGet]
        public IActionResult GetPendingChartofAccount()
        {
            return Ok(_rpRepository.GetPendingChartofAccount());
        }

        [Route("api.bankmodel/[controller]/pending-profile")]
        [HttpGet]
        public IActionResult GetPendingProfile()
        {
            return Ok(_rpRepository.GetPendingProfile());
        }

        [Route("api.bankmodel/[controller]/pending-account")]
        [HttpGet]
        public IActionResult GetPendingAccount()
        {
            return Ok(_rpRepository.GetPendingAccount());
        }

        [Route("api.bankmodel/[controller]/pending-transaction")]
        [HttpGet]
        public IActionResult GetPendingTransaction()
        {
            return Ok(_rpRepository.GetPendingTransaction());
        }

        [Route("api.bankmodel/[controller]/pending-loan")]
        [HttpGet]
        public IActionResult GetPendingLoan()
        {
            return Ok(_rpRepository.GetPendingLoan());
        }

        [Route("api.bankmodel/[controller]/pending-loan-repayment")]
        [HttpGet]
        public IActionResult GetPendingLoanRepayment()
        {
            return Ok(_rpRepository.GetPendingLoanRepayment());
        }

        [Route("api.bankmodel/[controller]/pending-fixed-deposit")]
        [HttpGet]
        public IActionResult GetPendingFixedDeposit()
        {
            return Ok(_rpRepository.GetPendingFixedDeposit());
        }

        [Route("api.bankmodel/[controller]/pending-salary")]
        [HttpGet]
        public IActionResult GetPendingSalary()
        {
            return Ok(_rpRepository.GetPendingSalary());
        }

        [Route("api.bankmodel/[controller]/pending-mobile-money")]
        [HttpGet]
        public IActionResult GetPendingMobileMoney()
        {
            return Ok(_rpRepository.GetPendingMobileMoney());
        }

        #endregion


        //Handle EOD transactions
        #region
        [Route("api.bankmodel/[controller]/current-transaction-date")]
        [HttpGet]
        public IActionResult GetTransactionDate()
        {
            return Ok(_rpRepository.GetTransactionDate());
        }

        [Route("api.bankmodel/[controller]/eod-pending-items")]
        [HttpGet]
        public IActionResult NoEODPendingItems()
        {
            return Ok(_rpRepository.NoEODPendingItems());
        }

        [Route("api.bankmodel/[controller]/eod-processing")]
        [HttpPost]
        public async Task<IActionResult> EOD(EODViewModel model)
        {
            var result = await _rpRepository.EOD(model);
            if(result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        #endregion

        //Handle EOM transactions
        #region
        [Route("api.bankmodel/[controller]/eom-lastday")]
        [HttpGet]
        public IActionResult IsEOMLastDay()
        {
            return Ok(_rpRepository.IsEOMLastDay());
        }

        [Route("api.bankmodel/[controller]/eom-lastday-completed")]
        [HttpGet]
        public IActionResult EOMLastDayCompleted()
        {
            return Ok(_rpRepository.EOMLastDayCompleted());
        }

        [Route("api.bankmodel/[controller]/eom-completed")]
        [HttpGet]
        public IActionResult EOMCompleted()
        {
            return Ok(_rpRepository.EOMCompleted());
        }

        [Route("api.bankmodel/[controller]/eom-lastday-savings-interest")]
        [HttpPost]
        public IActionResult EOMLastDaySavingsInterest(string username)
        {
            var result = _rpRepository.EOMLastDaySavingsInterest(username);
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/eom-lastday-overdrawn")]
        [HttpPost]
        public IActionResult EOMLastDayOverdrawn()
        {
            var result = _rpRepository.EOMLastDayOverdrawn();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/eom-savings-interest")]
        [HttpPost]
        public async Task<IActionResult> EOMSavingsInterest()
        {
            var result = await _rpRepository.EOMSavingsInterest();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/eom-overdrawn-account")]
        [HttpPost]
        public async Task<IActionResult> EOMOverdrawnAccount()
        {
            var result = await _rpRepository.EOMOverdrawnAccount();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/eom-sms")]
        [HttpPost]
        public async Task<IActionResult> EOMSMS()
        {
            var result = await _rpRepository.EOMSMS();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/eom-loan-repayment")]
        [HttpPost]
        public async Task<IActionResult> EOMLoanRepayment()
        {
            var result = await _rpRepository.EOMLoanRepayment();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/eom-loan-default")]
        [HttpPost]
        public async Task<IActionResult> EOMLoanDefault()
        {
            var result = await _rpRepository.EOMLoanDefault();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/eom-fixed-deposit")]
        [HttpPost]
        public async Task<IActionResult> EOMFixedDeposit()
        {
            var result = await _rpRepository.EOMFixedDeposit();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/eom-standing-order")]
        [HttpPost]
        public async Task<IActionResult> EOMStandingOrder()
        {
            var result = await _rpRepository.EOMStandingOrder();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/profit-and-loss")]
        [HttpPost]
        public async Task<IActionResult> EOMProfitandLoss()
        {
            var result = await _rpRepository.EOMProfitandLoss();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/balance-sheet")]
        [HttpPost]
        public async Task<IActionResult> EOMBalanceSheet()
        {
            var result = await _rpRepository.EOMBalanceSheet();
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [Route("api.bankmodel/[controller]/eom-lastday-overdrawn")]
        [HttpPost]
        public async Task<IActionResult> StartofMonth(EODViewModel model)
        {
            var result = await _rpRepository.StartofMonth(model);
            if (result == "Succeeded")
            {
                return Ok();
            }
            return BadRequest(result);
        }
        #endregion

    }
}
