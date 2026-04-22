using FluentValidation;
using SchoolManagement.Application.Teachers.Dtos;

namespace SchoolManagement.Application.Teachers.Validators;

public class CreateTeacherValidator : AbstractValidator<CreateTeacherDto>
{
    public CreateTeacherValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Email).MaximumLength(256).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
