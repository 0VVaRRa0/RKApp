using FluentValidation;
using ServerAPI.Dtos;

namespace ServerAPI.Validators;

public class InvoiceValidator : AbstractValidator<InvoiceDto>
{
    private const int AmountMinValue = 0;
    private const int ReceiptNumberMaxLength = 50;
    public InvoiceValidator()
    {
        RuleFor(inv => inv.ClientId)
            .NotEmpty()
            .WithMessage("Поле ClientId обязательно для заполнения");

        RuleFor(inv => inv.ServiceId)
            .NotEmpty()
            .WithMessage("Поле ServiceId обязательно для заполнения");

        RuleFor(inv => inv.Amount)
        .GreaterThan(AmountMinValue)
        .WithMessage($"Сумма должна быть больше {AmountMinValue}");

        RuleFor(inv => inv.IssueDate)
            .NotEmpty()
            .WithMessage("Дата выставления обязательна для заполнения");

        RuleFor(inv => inv.IssueDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .When(inv => inv.Id == 0)
            .WithMessage("Дата выставления не может быть раньше, чем сегодня");

        RuleFor(inv => inv.DueDate)
            .NotEmpty()
            .WithMessage("Дата оплаты обязательна для заполнения")
            .GreaterThanOrEqualTo(inv => inv.IssueDate)
            .WithMessage("Дата оплаты не может быть раньше даты выставления");

        RuleFor(inv => inv.PaymentDate)
            .GreaterThanOrEqualTo(inv => inv.IssueDate)
            .When(inv => inv.PaymentDate.HasValue)
            .WithMessage("Дата оплаты не может быть раньше даты выставления");

        RuleFor(inv => inv.ReceiptNumber)
            .MaximumLength(ReceiptNumberMaxLength)
            .When(inv => !string.IsNullOrEmpty(inv.ReceiptNumber))
            .WithMessage($"Номер квитанции не должен превышать {ReceiptNumberMaxLength} символов");
    }
}
