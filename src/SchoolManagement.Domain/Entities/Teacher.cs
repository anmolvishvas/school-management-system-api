namespace SchoolManagement.Domain.Entities;

public class Teacher
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
}
