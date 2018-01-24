using Microsoft.AspNetCore.Mvc.ModelBinding;
using BankModel.Web.Interfaces;

namespace BankModel.Web.Services
{
    public class ModelStateWrapper: IValidationDictionary
    {
        ModelStateDictionary _modelState;

        public ModelStateWrapper(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        public bool IsValid
        {
            get { return _modelState.IsValid; }
        }

        public void AddError(string key, string errorMessage)
        {
            _modelState.AddModelError(key, errorMessage);
        }
    }
}
