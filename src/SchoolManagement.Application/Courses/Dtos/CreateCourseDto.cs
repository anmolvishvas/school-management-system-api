namespace SchoolManagement.Application.Courses.Dtos;

public class CreateCourseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
}
