namespace SchoolManagement.Application.PeriodAttendance.Dtos;

public class BulkMarkPeriodAttendanceDto
{
    public DateOnly Date { get; set; }
    public int HourNumber { get; set; }
    public int SubjectId { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public List<PeriodAttendanceLineDto> Lines { get; set; } = new();
}
