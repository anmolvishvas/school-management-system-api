using FluentValidation;
using SchoolManagement.Application.Timetables.Dtos;

namespace SchoolManagement.Application.Timetables.Validators;

public class CreateTimetableEntryValidator : AbstractValidator<CreateTimetableEntryDto>
{
    public CreateTimetableEntryValidator()
    {
        RuleFor(x => x.Class).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DayOfWeek).IsInEnum();
        RuleFor(x => x.EndTime)
            .Must((dto, end) => end > dto.StartTime)
            .WithMessage("EndTime must be greater than StartTime.");
        RuleFor(x => x.SubjectId).GreaterThan(0);
        RuleFor(x => x.TeacherId).GreaterThan(0);
    }
}
