using FluentValidation;
using SchoolManagement.Application.PeriodAttendance.Dtos;

namespace SchoolManagement.Application.PeriodAttendance.Validators;

public class UpdateSubjectValidator : AbstractValidator<UpdateSubjectDto>
{
    public UpdateSubjectValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
