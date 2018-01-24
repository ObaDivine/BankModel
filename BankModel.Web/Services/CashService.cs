using BankModel.Service.Interfaces;
using BankModel.Web.ViewModels;
using BankModel.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankModel.Web.Services
{
    public class CashService: ICashService
    {
        private IValidationDictionary _validationDictionary;
        private readonly IConfiguration _config;
        private readonly ICashRepository _cashRepository;

        public CashService(ISystemAdminRepository systemAdminRepository, ICashRepository cashRepository, IValidationDictionary validationDictionary, IConfiguration config)
        {
            _cashRepository = cashRepository;
            _validationDictionary = validationDictionary;
            _config = config;
        }

        public string GetTellerAccount(string username)
        {
            return _cashRepository.GetTellerAccount(username);
        }

        //Deposit transactions
        #region
        public async Task<List<string>> ValidateCashDepositRequirement(TransactionViewModel model, string username)
        {
            //This checks if the credit account exist
            if (_cashRepository.CheckIfAccountExist(model.CR) == null)
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["InvalidAccount"],"Credit ", model.CR));

            //This checks if the account is active or not
            if (_cashRepository.CheckAccountStatus(model.CR) != "ACTIVE")
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["DisabledAccount"], model.CR));

            //This checks for same account
            if (model.DR == model.CR)
                _validationDictionary.AddError(_config.GetSection("Messages")["SameAccount"]);

            //This checks post no credit on the customer account
            if (_cashRepository.CheckPostNoCredit(model.CR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["PostNoCredit"], model.CR));

            //This checks the transaction limit set on the account
            if (_cashRepository.CheckCashDepositAmountLimitReached(model.CR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["TransactionLimitReached"], model.CR));

            //This checks the transaction limit set on the account
            if (_cashRepository.CheckCashDepositTransactionLimitReached(model.CR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["TransactionLimitReached"], model.CR));

            //This checks the limit of cash transaction allowed for the user
            if (_cashRepository.TransactionLimitReached(username, model.Amount))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["PostingLimitReached"], model.Amount));

            //This checks if Interbranch transaction
            string debitBranch = _cashRepository.GetBranchCodeFromAccountNo(model.DR);
            string creditBranch = _cashRepository.GetBranchCodeFromAccountNo(model.CR);
            if (debitBranch != creditBranch)
            {
                if (!_cashRepository.InterBranchAccountExist(model.DR, model.CR))
                    _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["NoInterBranchAccount"], model.CR));

                //This checks if user has Inter Branch role
                bool userHasRole = await _cashRepository.CheckIfUserHasRole(username);
                if (!userHasRole)
                    _validationDictionary.AddError(_config.GetSection("Messages")["NoInterBranchRole"]);

                //This checks if the liability account in the destination branch is created
                if (_cashRepository.GetLiabilityAccountNo(_cashRepository.GetProductCodeFromAccountNo(model.CR), _cashRepository.GetBranchCodeFromAccountNo(model.CR)) == null)
                    _validationDictionary.AddError(_config.GetSection("Messages")["NoLiabilityAccount"]);
            }

            //This checks if the user is a Teller (Has a Teller account) in the form TELLER USERNAME
            if (!_cashRepository.CheckIfTeller(username))
                _validationDictionary.AddError(_config.GetSection("Messages")["NotTeller"]);

            return _validationDictionary.GetValidationErrors();
        }

        public async Task<string> CreateCashDepositAsync(TransactionViewModel model, string username)
        {
            var result = await _cashRepository.CreateCashDepositAsync(model, username);
            if (result.Equals("Succeeded"))
                return "Succeeded";
            else
                return "Failed";
        }
        #endregion

        //Withdrawal transactions
        #region
        public async Task<List<string>> ValidateCashWithdrawalRequirement(TransactionViewModel model, string username)
        {

            //This checks if the debit account exist
            if (_cashRepository.CheckIfAccountExist(model.DR) == null)
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["InvalidAccount"], "Debit ", model.DR));

            //This checks for same account
            if (model.DR == model.CR)
                _validationDictionary.AddError(_config.GetSection("Messages")["SameAccount"]);

            //This checks if the customer account is funded
            if (!_cashRepository.CheckIfAccountIsFunded("CUSTOMER", model.DR, model.Amount))
            {
                //Check if overdrwan is allowed
                if(!_cashRepository.AllowOverdraw(model.DR))
                    _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["InsufficientBalance"], model.DR));
            }

            //This checks if the teller account is funded
            if (!_cashRepository.CheckIfAccountIsFunded("TELLER", model.CR, model.Amount))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["InvalidAccount"], model.CR));

            //This checks transaction limits
            if (_cashRepository.CheckPostNoDebit(model.DR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["PostNoDebit"], model.DR));

            //This checks if the account is active or not
            if (_cashRepository.CheckAccountStatus(model.DR) != "ACTIVE")
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["DisabledAccount"], model.DR));

            //This checks the transaction limit set on the account
            if (_cashRepository.CheckCashWithdrawalAmountLimitReached(model.DR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["TransactionLimitReached"], model.CR));

            //This checks the transaction limit set on the account
            if (_cashRepository.CheckCashWithdrawalTransactionLimitReached(model.DR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["TransactionLimitReached"], model.DR));

            //This checks the limit of cash transaction allowed for the user
            if (_cashRepository.TransactionLimitReached(username, model.Amount))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["PostingLimitReached"], model.Amount));

            //This checks if Interbranch transaction
            string debitBranch = _cashRepository.GetBranchCodeFromAccountNo(model.DR);
            string creditBranch = _cashRepository.GetBranchCodeFromAccountNo(model.CR);
            if (debitBranch != creditBranch)
            {
                if (!_cashRepository.InterBranchAccountExist(model.CR, model.DR))
                    _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["NoInterBranchAccount"], model.DR));

                //This checks if user has Inter Branch role
                bool userHasRole = await _cashRepository.CheckIfUserHasRole(username);
                if (!userHasRole)
                    _validationDictionary.AddError(_config.GetSection("Messages")["NoInterBranchRole"]);

                //This checks if the liability account in the destination branch is created
                if (_cashRepository.GetLiabilityAccountNo(_cashRepository.GetProductCodeFromAccountNo(model.DR), _cashRepository.GetBranchCodeFromAccountNo(model.DR)) == null)
                    _validationDictionary.AddError(_config.GetSection("Messages")["NoLiabilityAccount"]);
            }

            //This checks if the user is a Teller (Has a Teller account) in the form TELLER USERNAME
            if (!_cashRepository.CheckIfTeller(username))
                _validationDictionary.AddError(_config.GetSection("Messages")["NotTeller"]);

            return _validationDictionary.GetValidationErrors();

        }
        public async Task<string> CreateCashWithdrawalAsync(TransactionViewModel model, string username)
        {
            var result = await _cashRepository.CreateCashWithdrawalAsync(model, username);
            if (result.Equals("Succeeded"))
                return "Succeeded";
            else
                return "Failed";
        }
        #endregion

        //Fund transfer transactions
        #region
        public async Task<List<string>> ValidateFundTransferRequirement(TransactionViewModel model, string username)
        {
            //This checks if the debit account exist
            if (_cashRepository.CheckIfAccountExist(model.DR) == null)
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["InvalidAccount"], "Debit ", model.DR));

            //This checks if the credit account exist
            if (_cashRepository.CheckIfAccountExist(model.CR) == null)
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["InvalidAccount"], "Credit ", model.CR));

            //This checks for same account
            if (model.DR == model.CR)
                _validationDictionary.AddError(_config.GetSection("Messages")["SameAccount"]);

            //This checks if the debit account is funded
            if (!_cashRepository.CheckIfAccountIsFunded("CUSTOMER", model.DR, model.Amount))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["InsufficientBalance"], model.DR));

            //This checks post no credit and debit flag
            if (_cashRepository.CheckPostNoCredit(model.CR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["PostNoCredit"], model.CR));

            if (_cashRepository.CheckPostNoDebit(model.DR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["PostNoDebit"], model.DR));

            //This checks if the account is active or not
            if (_cashRepository.CheckAccountStatus(model.DR) != "ACTIVE")
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["DisabledAccount"], model.DR));

            //This checks if the account is active or not
            if (_cashRepository.CheckAccountStatus(model.CR) != "ACTIVE")
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["DisabledAccount"], model.CR));

            //This checks the transaction limit set on the account
            if (_cashRepository.CheckCashDepositAmountLimitReached(model.CR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["TransactionLimitReached"], model.CR));

            //This checks the transaction limit set on the account
            if (_cashRepository.CheckCashDepositTransactionLimitReached(model.CR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["TransactionLimitReached"], model.CR));

            //This checks the transaction limit set on the account
            if (_cashRepository.CheckCashWithdrawalAmountLimitReached(model.DR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["TransactionLimitReached"], model.CR));

            //This checks the transaction limit set on the account
            if (_cashRepository.CheckCashWithdrawalTransactionLimitReached(model.DR))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["TransactionLimitReached"], model.DR));

            //This checks the limit of cash transaction allowed for the user
            if (_cashRepository.TransactionLimitReached(username, model.Amount))
                _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["PostingLimitReached"], model.Amount));

            //This checks if Interbranch transaction
            string debitBranch = _cashRepository.GetBranchCodeFromAccountNo(model.DR);
            string creditBranch = _cashRepository.GetBranchCodeFromAccountNo(model.CR);
            if (debitBranch != creditBranch)
            {
                if (!_cashRepository.InterBranchAccountExist(model.DR, model.CR))
                    _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["NoInterBranchAccount"], model.CR));

                if (!_cashRepository.InterBranchAccountExist(model.CR, model.DR))
                    _validationDictionary.AddError(string.Format(_config.GetSection("Messages")["NoInterBranchAccount"], model.DR));

                //This checks if user has Inter Branch role
                bool userHasRole = await _cashRepository.CheckIfUserHasRole(username);
                if (!userHasRole)
                    _validationDictionary.AddError(_config.GetSection("Messages")["NoInterBranchRole"]);

                //This checks if the liability account in the destination branch is created
                if (_cashRepository.GetLiabilityAccountNo(_cashRepository.GetProductCodeFromAccountNo(model.DR), _cashRepository.GetBranchCodeFromAccountNo(model.DR)) == null)
                    _validationDictionary.AddError(_config.GetSection("Messages")["NoLiabilityAccount"]);

                //This checks if the liability account in the destination branch is created
                if (_cashRepository.GetLiabilityAccountNo(_cashRepository.GetProductCodeFromAccountNo(model.CR), _cashRepository.GetBranchCodeFromAccountNo(model.CR)) == null)
                    _validationDictionary.AddError(_config.GetSection("Messages")["NoLiabilityAccount"]);
            }

            //This checks if the user is a Teller (Has a Teller account) in the form TELLER USERNAME
            if (!_cashRepository.CheckIfTeller(username))
                _validationDictionary.AddError(_config.GetSection("Messages")["NotTeller"]);

            return _validationDictionary.GetValidationErrors();
        }
        public async Task<string> CreateFundTransferAsync(TransactionViewModel model, string username)
        {
            var result = await _cashRepository.CreateFundTransferAsync(model, username);
            if (result.Equals("Succeeded"))
                return "Succeeded";
            else
                return "Failed";
        }
        #endregion
    }
}
