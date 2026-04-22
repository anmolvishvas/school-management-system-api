namespace SchoolManagement.Application.Teachers.Dtos;

public class CreateTeacherDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
}
