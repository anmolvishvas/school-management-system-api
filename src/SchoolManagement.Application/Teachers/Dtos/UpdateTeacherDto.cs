namespace SchoolManagement.Application.Teachers.Dtos;

public class UpdateTeacherDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
}
