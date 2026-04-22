using FluentValidation;
using SchoolManagement.Application.Courses.Dtos;

namespace SchoolManagement.Application.Courses.Validators;

public class UpdateCourseValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Code).MaximumLength(30);
    }
}
