using FluentValidation;
using SchoolManagement.Application.Teachers.Dtos;

namespace SchoolManagement.Application.Teachers.Validators;

public class UpdateTeacherValidator : AbstractValidator<UpdateTeacherDto>
{
    public UpdateTeacherValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Email).MaximumLength(256).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
