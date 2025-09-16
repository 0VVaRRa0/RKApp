namespace Accounting.Validation;

public class InvoiceValidation
{
    private ErrorProvider _errorProvider = new();

    public bool Validate(ComboBox service, ComboBox client, NumericUpDown amount)
    {
        _errorProvider.Clear();
        bool isValid = true;

        if (service.SelectedValue == null)
        {
            _errorProvider.SetError(service, "Выберите услугу");
            isValid = false;
        }
        if (client.SelectedValue == null)
        {
            _errorProvider.SetError(client, "Выберите клиента");
            isValid = false;
        }
        if (amount.Value == 0)
        {
            _errorProvider.SetError(amount, "Сумма не может быть равна нулю");
            isValid = false;
        }

        return isValid;
    }
}
