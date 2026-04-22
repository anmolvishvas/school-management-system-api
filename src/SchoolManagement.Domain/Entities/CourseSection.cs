namespace SchoolManagement.Domain.Entities;

public class CourseSection
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public string Section { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
