using FluentValidation;
using SchoolManagement.Application.PeriodAttendance.Dtos;

namespace SchoolManagement.Application.PeriodAttendance.Validators;

public class BulkMarkPeriodAttendanceValidator : AbstractValidator<BulkMarkPeriodAttendanceDto>
{
    public BulkMarkPeriodAttendanceValidator()
    {
        RuleFor(x => x.HourNumber).InclusiveBetween(1, 12);
        RuleFor(x => x.SubjectId).GreaterThan(0);
        RuleFor(x => x.Class).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.StudentId).GreaterThan(0);
            line.RuleFor(x => x.Status).IsInEnum();
            line.RuleFor(x => x.Notes).MaximumLength(500);
        });
    }
}
