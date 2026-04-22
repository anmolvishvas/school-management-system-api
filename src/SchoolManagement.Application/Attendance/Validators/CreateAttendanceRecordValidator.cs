using FluentValidation;
using SchoolManagement.Application.Attendance.Dtos;

namespace SchoolManagement.Application.Attendance.Validators;

public class CreateAttendanceRecordValidator : AbstractValidator<CreateAttendanceRecordDto>
{
    public CreateAttendanceRecordValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0);
        RuleFor(x => x.Class).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
