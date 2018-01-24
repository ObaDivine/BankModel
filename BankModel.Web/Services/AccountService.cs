using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using BankModel.Web.ViewModels;
using BankModel.Web.Interfaces;

namespace BankModel.Web.Services
{
    public class AccountService: IAccountService
    {
        //private readonly DBContext _context;
        private readonly IAccountRepository _accountRepository;
        private IValidationDictionary _validationDictionary;
        private readonly IConfiguration _config;
        public AccountService(DBContext context, IAccountRepository accountRepository, IValidationDictionary validationDictionary, IConfiguration config)
        {
            //_context = context;
            _accountRepository = accountRepository;
            _validationDictionary = validationDictionary;
            _config = config;
        }

        public string GetUserStatus(string username)
        {
            return _accountRepository.GetUserStatus(username);
        }

        public DateTime GetPasswordExpiryDate(string username)
        {
            return _accountRepository.GetPasswordExpiryDate(username);
        }

        public string GetProfileImage(string username)
        {
            return _accountRepository.GetProfileImage(username);
        }

        public string GetUserRole(string username)
        {
            return _accountRepository.GetUserRole(username);
        }

        public string GetUserBranch(string username)
        {
            return _accountRepository.GetUserBranch(username);
        }

        public string GetTransactionDate()
        {
            return _accountRepository.GetTransactionDate();
        }

        public List<string> ValidateLoginRequirement(LoginViewModel model)
        {
            //Check if the user password is expired
            if (DateTime.UtcNow.Date >= _accountRepository.GetPasswordExpiryDate(model.Username))
            {
                _validationDictionary.AddError(_config.GetSection("Messages")["ExpiredPassword"]);
            }

            //Check if End of day is in progress
            if (_accountRepository.GetUserStatus(model.Username).Contains("EOD"))
            {
                _validationDictionary.AddError(_config.GetSection("Messages")["EODInProgress"]);
            }

            //Check if the user is Active or not
            if (_accountRepository.GetUserStatus(model.Username) == "DISABLED" || _accountRepository.GetUserStatus(model.Username) == "PENDING")
            {
                _validationDictionary.AddError(_config.GetSection("Messages")["UserDisabled"]);
            }

            //This checks if the user is logged in already
            if (_accountRepository.UserIsLoggedIn(model.Username))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["UserLoggedIn"], model.Username));

            return _validationDictionary.GetValidationErrors();
        }

        public void SetUserLogin(string username)
        {
            _accountRepository.SetUserLogin(username);
        }

        public void ClearUserLogin(string username)
        {
            _accountRepository.ClearUserLogin(username);
        }
    }
}
