namespace BankModel.Web.Interfaces
{
    public interface IValidationDictionary
    {
        bool IsValid{ get; }
        void AddError(string key, string errorMessage);
    }
}
