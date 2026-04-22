using FluentValidation;
using SchoolManagement.Application.Attendance.Dtos;

namespace SchoolManagement.Application.Attendance.Validators;

public class BulkMarkAttendanceDayValidator : AbstractValidator<BulkMarkAttendanceDayDto>
{
    public BulkMarkAttendanceDayValidator()
    {
        RuleFor(x => x.Class).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Lines).NotEmpty();
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.StudentId).GreaterThan(0);
            line.RuleFor(l => l.Status).IsInEnum();
            line.RuleFor(l => l.Notes).MaximumLength(500);
        });
    }
}
