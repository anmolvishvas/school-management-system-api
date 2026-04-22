namespace SchoolManagement.Application.Courses.Dtos;

public class CourseSectionDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Section { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
