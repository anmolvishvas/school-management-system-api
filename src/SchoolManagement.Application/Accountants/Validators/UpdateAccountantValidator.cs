using FluentValidation;
using SchoolManagement.Application.Accountants.Dtos;

namespace SchoolManagement.Application.Accountants.Validators;

public class UpdateAccountantValidator : AbstractValidator<UpdateAccountantDto>
{
    public UpdateAccountantValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Email).MaximumLength(256);
    }
}
