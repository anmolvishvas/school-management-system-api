using FluentValidation;
using SchoolManagement.Application.Auth.Dtos;

namespace SchoolManagement.Application.Auth.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(200);
    }
}
