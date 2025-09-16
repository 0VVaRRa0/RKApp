using System.Text.RegularExpressions;

namespace Accounting.Validation;

public class ClientValidation
{
    private ErrorProvider _errorProvider = new();
    private const int LoginMinLength = 3;
    private const int FullNameMinLength = 5;

    public bool Validate(TextBox loginTB, TextBox fullNameTB, TextBox emailTB, TextBox phoneTB)
    {
        _errorProvider.Clear();
        bool isValid = true;
        string login = loginTB.Text.Trim();
        string fullName = fullNameTB.Text.Trim();
        string email = emailTB.Text.Trim();
        string phone = phoneTB.Text.Trim();

        if (login.Length < LoginMinLength)
        {
            _errorProvider.SetError(
                loginTB, $"Логин должен быть не меньше {LoginMinLength} символов");
            isValid = false;
        }

        if (fullName.Length < FullNameMinLength)
        {
            _errorProvider.SetError(
                fullNameTB, $"Имя клиента должно быть не меньше {FullNameMinLength} символов");
            isValid = false;
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            _errorProvider.SetError(emailTB, "Неверно указан формат email");
            isValid = false;
        }

        if (!Regex.IsMatch(phone, @"^\+\d{10,20}$"))
        {
            _errorProvider.SetError(phoneTB, "Неверно указан формат телефона");
            isValid = false;
        }

        return isValid;
    }
}
