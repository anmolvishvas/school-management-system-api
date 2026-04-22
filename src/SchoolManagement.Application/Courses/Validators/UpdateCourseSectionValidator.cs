using FluentValidation;
using SchoolManagement.Application.Courses.Dtos;

namespace SchoolManagement.Application.Courses.Validators;

public class UpdateCourseSectionValidator : AbstractValidator<UpdateCourseSectionDto>
{
    public UpdateCourseSectionValidator()
    {
        RuleFor(x => x.Section).NotEmpty().MaximumLength(50);
    }
}
