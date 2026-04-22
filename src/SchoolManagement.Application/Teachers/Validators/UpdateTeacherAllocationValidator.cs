using FluentValidation;
using SchoolManagement.Application.Teachers.Dtos;

namespace SchoolManagement.Application.Teachers.Validators;

public class UpdateTeacherAllocationValidator : AbstractValidator<UpdateTeacherAllocationDto>
{
    public UpdateTeacherAllocationValidator()
    {
        RuleFor(x => x.SubjectId).GreaterThan(0);
        RuleFor(x => x.Class).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
    }
}
