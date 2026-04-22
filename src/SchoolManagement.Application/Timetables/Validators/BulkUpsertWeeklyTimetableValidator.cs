using FluentValidation;
using SchoolManagement.Application.Timetables.Dtos;

namespace SchoolManagement.Application.Timetables.Validators;

public class BulkUpsertWeeklyTimetableValidator : AbstractValidator<BulkUpsertWeeklyTimetableDto>
{
    public BulkUpsertWeeklyTimetableValidator()
    {
        RuleFor(x => x.Class).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Lines).NotNull().NotEmpty();

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(x => x.DayOfWeek).IsInEnum();
            line.RuleFor(x => x.EndTime)
                .Must((dto, end) => end > dto.StartTime)
                .WithMessage("EndTime must be greater than StartTime.");
            line.RuleFor(x => x.SubjectId).GreaterThan(0);
            line.RuleFor(x => x.TeacherId).GreaterThan(0);
        });
    }
}
