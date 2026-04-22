using FluentValidation;
using SchoolManagement.Application.Attendance.Dtos;

namespace SchoolManagement.Application.Attendance.Validators;

public class UpdateAttendanceRecordValidator : AbstractValidator<UpdateAttendanceRecordDto>
{
    public UpdateAttendanceRecordValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
