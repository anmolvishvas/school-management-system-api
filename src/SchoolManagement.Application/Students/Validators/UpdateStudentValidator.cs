using FluentValidation;
using SchoolManagement.Application.Students.Dtos;

namespace SchoolManagement.Application.Students.Validators;

public class UpdateStudentValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Class).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).MaximumLength(256).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.ParentGuardianEmail).MaximumLength(256).EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.ParentGuardianEmail));
    }
}
