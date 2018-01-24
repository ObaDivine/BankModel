using BankModel.Models;
using BankModel.Models.ViewModels;
using BankModel.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace BankModel.Web.Controllers
{
    [Authorize(Policy = "Enquiry")]
    public class EnquiryController : Controller
    {
        private readonly IEnquiryService _enquiryService;
        public EnquiryController(IEnquiryService enquiryService)
        {
            _enquiryService = enquiryService;
        }

        #region
        [HttpGet]
        [Authorize(Policy = "CustomerEnquiry")]
        public IActionResult CustomerEnquiry()
        {
            ViewBag.CustomerAccounts = _enquiryService.GetCustomerAccounts(string.Empty);
            return View(new EnquiryViewModel());
        }

        [HttpPost]
        [Authorize(Policy = "CustomerEnquiry")]
        public IActionResult CustomerEnquiry(string accountNo = null)
        {
            //Get all other accounts belonging to the same customer
            ViewBag.CustomerAccounts = _enquiryService.GetCustomerAccounts(accountNo);
            var enquiryDetails = _enquiryService.GetCustomerAccountDetails(accountNo);

            if (enquiryDetails != null)
                return View(enquiryDetails);
            else
                return View(new EnquiryViewModel());
        }

        [Authorize(Policy = "CustomerEnquiry")]
        public IActionResult CustomerProfile(string ID = null)
        {
            if(string.IsNullOrEmpty(ID))
            {
                return View(new Profile());
            }
            return View(_enquiryService.GetCustomerProfile(ID));
        }

        [Authorize(Policy = "CustomerEnquiry")]
        public IActionResult AccountTransactions(string accountNo = null)
        {
            return View(_enquiryService.GetAccountTransactions(accountNo));
        }

        #endregion

        #region
        [HttpGet]
        [Authorize(Policy = "GLEnquiry")]
        public IActionResult GLEnquiry()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        #endregion

    }
}
