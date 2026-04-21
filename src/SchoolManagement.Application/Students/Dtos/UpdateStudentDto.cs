namespace SchoolManagement.Application.Students.Dtos;

public class UpdateStudentDto
{
    public string Name { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? ParentGuardianName { get; set; }
    public string? ParentGuardianPhone { get; set; }
    public string? ParentGuardianEmail { get; set; }
    public string? AdmissionNumber { get; set; }
    public DateOnly? DateOfAdmission { get; set; }
    public string? EmergencyContact { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
}
