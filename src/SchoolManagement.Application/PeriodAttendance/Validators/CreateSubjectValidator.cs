using FluentValidation;
using SchoolManagement.Application.PeriodAttendance.Dtos;

namespace SchoolManagement.Application.PeriodAttendance.Validators;

public class CreateSubjectValidator : AbstractValidator<CreateSubjectDto>
{
    public CreateSubjectValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
