namespace SchoolManagement.Application.PeriodAttendance.Dtos;

public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
