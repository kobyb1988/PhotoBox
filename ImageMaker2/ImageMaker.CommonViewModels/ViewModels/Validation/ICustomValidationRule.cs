namespace ImageMaker.CommonViewModels.ViewModels.Validation
{
    public interface ICustomValidationRule
    {
        string GetError(string propertyName);

        bool IsValidObject { get; }
    }
}