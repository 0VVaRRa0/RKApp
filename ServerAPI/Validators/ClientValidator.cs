using FluentValidation;
using ServerAPI.Dtos;

namespace ServerAPI.Validators;

public class ClientValidator : AbstractValidator<ClientDto>
{
    private const int LoginMinLength = 3;
    private const int FullNameMinLength = 5;
    public ClientValidator()
    {
        RuleFor(c => c.Login)
            .NotEmpty()
            .WithMessage("Логин обязателен для заполнения")
            .MinimumLength(LoginMinLength)
            .WithMessage($"Логин должен быть не меньше {LoginMinLength} символов");

        RuleFor(c => c.FullName)
            .NotEmpty()
            .WithMessage("ФИО обязательно для заполнения")
            .MinimumLength(5)
            .WithMessage($"ФИО должно быть не меньше {FullNameMinLength} символов");

        RuleFor(c => c.Phone)
            .NotEmpty()
            .WithMessage("Телефон обязателен для заполнения")
            .Matches(@"^\+\d{10,20}$")
            .WithMessage("Телефон должен состоять только из цифр и должен начинаться начинаться с '+'");

        RuleFor(c => c.Email)
            .NotEmpty()
            .WithMessage("Email обязателен для заполнения")
            .EmailAddress()
            .WithMessage("Некорректный формат email");
    }
}
