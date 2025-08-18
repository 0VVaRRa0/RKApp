using FluentValidation;
using ServerAPI.Dtos;

namespace ServerAPI.Validators;

public class ServiceValidator : AbstractValidator<ServiceDto>
{
    public ServiceValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty()
            .WithMessage("Название услуги обязательно для заполнения")
            .MinimumLength(3)
            .WithMessage("Название услуги должно быть не меньше 3 символов");
    }
}
