namespace SchoolManagement.Application.Courses.Dtos;

public class UpdateCourseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
}
