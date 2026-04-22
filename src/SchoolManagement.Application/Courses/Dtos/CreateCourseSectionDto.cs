namespace SchoolManagement.Application.Courses.Dtos;

public class CreateCourseSectionDto
{
    public string Section { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
