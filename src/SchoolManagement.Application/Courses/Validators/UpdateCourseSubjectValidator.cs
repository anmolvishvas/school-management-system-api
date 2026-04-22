using FluentValidation;
using SchoolManagement.Application.Courses.Dtos;

namespace SchoolManagement.Application.Courses.Validators;

public class UpdateCourseSubjectValidator : AbstractValidator<UpdateCourseSubjectDto>
{
    public UpdateCourseSubjectValidator()
    {
        RuleFor(x => x.SubjectId).GreaterThan(0);
    }
}
