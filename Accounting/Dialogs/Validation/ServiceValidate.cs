namespace Accounting.Dialogs.Validation;

public class ServiceValidation
{
    private ErrorProvider _errorProvider = new();
    private const int ServiceNameMinLength = 3;

    public bool Validate(TextBox nameTB)
    {
        _errorProvider.Clear();
        bool isValid = true;
        string name = nameTB.Text.Trim();

        if (name.Length < ServiceNameMinLength)
        {
            _errorProvider.SetError(
                nameTB, $"Название должно быть не меньше {ServiceNameMinLength} символов");
            isValid = false;
        }

        return isValid;
    }
}
