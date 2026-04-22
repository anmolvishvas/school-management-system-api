namespace SchoolManagement.Application.Accountants.Dtos;

public class UpdateAccountantDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
}
