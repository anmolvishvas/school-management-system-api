using FluentValidation;
using SchoolManagement.Application.Courses.Dtos;

namespace SchoolManagement.Application.Courses.Validators;

public class CreateCourseSectionValidator : AbstractValidator<CreateCourseSectionDto>
{
    public CreateCourseSectionValidator()
    {
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
    }
}
