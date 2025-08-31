using FluentValidation;
using ServerAPI.Dtos;

namespace ServerAPI.Validators;

public class ServiceValidator : AbstractValidator<ServiceDto>
{
    private const int ServiceNameMinLength = 3;
    public ServiceValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty()
            .WithMessage("Название услуги обязательно для заполнения")
            .MinimumLength(ServiceNameMinLength)
            .WithMessage($"Название услуги должно быть не меньше {ServiceNameMinLength} символов");
    }
}
