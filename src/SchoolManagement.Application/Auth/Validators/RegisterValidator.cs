using FluentValidation;
using SchoolManagement.Application.Auth.Dtos;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Auth.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(200);
        RuleFor(x => x.Role).NotEmpty().Must(r =>
                ApplicationRoles.All.Any(a => string.Equals(a, r.Trim(), StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"Role must be one of: {string.Join(", ", ApplicationRoles.All)}");
    }
}
