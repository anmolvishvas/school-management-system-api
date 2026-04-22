using FluentValidation;
using SchoolManagement.Application.PeriodAttendance.Dtos;

namespace SchoolManagement.Application.PeriodAttendance.Validators;

public class BulkMarkPeriodFromTimetableValidator : AbstractValidator<BulkMarkPeriodFromTimetableDto>
{
    public BulkMarkPeriodFromTimetableValidator()
    {
        RuleFor(x => x.TimetableEntryId).GreaterThan(0);
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.StudentId).GreaterThan(0);
            line.RuleFor(x => x.Status).IsInEnum();
            line.RuleFor(x => x.Notes).MaximumLength(500);
        });
    }
}
