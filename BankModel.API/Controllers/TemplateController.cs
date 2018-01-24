using BankModel.Models.ViewModels;
using BankModel.Models;
using BankModel.Data.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BankModel.API.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ITemplateRepository _templateRepository;

        public TemplateController(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        // POST api/<controller>
        [Route("api.bankmodel/[controller]/createaccounttemplate")]
        [HttpPost]
        public async Task<IActionResult> CreateAccountTemplate(TemplateAccountViewModel model)
        {
            var result = await _templateRepository.CreateAccountTemplateAsync(model);
            if(result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        // PUT api/<controller>/5
        [Route("api.bankmodel/[controller]/updateaccounttemplate")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTemplate(int id, TemplateAccountViewModel model)
        {
            var result = await _templateRepository.UpdateAccountTemplateAsync(model);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        // DELETE api/<controller>/5
        [Route("api.bankmodel/[controller]/dropaccounttemplate")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var result = await _templateRepository.DropAccountTemplateAsync(id);
            if (result == "Successful")
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api.bankmodel/controller/accounttemplateexist")]
        [HttpGet("{id}")]
        public bool AccountTemplateExist(string id)
        {
            var result = _templateRepository.AccountTemplateExist(id);
            return (result == true ? true : false);
        }

        [Route("api.bankmodel/controller/productcodeexist")]
        [HttpGet("{id}")]
        public bool ProductCodeExist(string id)
        {
            var result = _templateRepository.ProductCodeExist(id);
            return (result == true ? true : false);
        }

        [Route("api.bankmodel/controller/accounttemplateinuse")]
        [HttpGet("{id}")]
        public bool IsAccountTemplateInUse(int id)
        {
            var result = _templateRepository.IsAccountTemplateInUse(id);
            return (result == true ? true : false);
        }

        [Route("api.bankmodel/controller/accounttemplatebyuser")]
        [HttpGet("{id}")]
        public IEnumerable<TemplateAccount> GetAccountTemplateByUser(string id)
        {
            return _templateRepository.GetAccountTemplate(id);
        }

        [Route("api.bankmodel/controller/accounttemplatedetails")]
        [HttpGet("{id}")]
        public TemplateAccount GetAccountTemplateDetails(int id)
        {
            return _templateRepository.GetAccountTemplateDetails(id);
        }

        [Route("api.bankmodel/controller/accounttemplatebyid")]
        [HttpGet("{id}")]
        public TemplateAccountViewModel GetAccountTemplateByID(int id)
        {
            return _templateRepository.GetAccountTemplateByID(id);
        }
    }
}
