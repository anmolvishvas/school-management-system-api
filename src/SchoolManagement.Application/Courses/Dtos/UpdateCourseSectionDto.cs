namespace SchoolManagement.Application.Courses.Dtos;

public class UpdateCourseSectionDto
{
    public string Section { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
