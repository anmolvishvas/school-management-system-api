using FluentValidation;
using SchoolManagement.Application.Accountants.Dtos;

namespace SchoolManagement.Application.Accountants.Validators;

public class CreateAccountantValidator : AbstractValidator<CreateAccountantDto>
{
    public CreateAccountantValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Email).MaximumLength(256);
    }
}
