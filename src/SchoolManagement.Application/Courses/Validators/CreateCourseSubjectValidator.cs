using FluentValidation;
using SchoolManagement.Application.Courses.Dtos;

namespace SchoolManagement.Application.Courses.Validators;

public class CreateCourseSubjectValidator : AbstractValidator<CreateCourseSubjectDto>
{
    public CreateCourseSubjectValidator()
    {
        RuleFor(x => x.SubjectId).GreaterThan(0);
    }
}
