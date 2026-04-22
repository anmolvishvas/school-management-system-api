namespace SchoolManagement.Application.Attendance.Dtos;

public class BulkMarkAttendanceDayDto
{
    public DateOnly Date { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public List<BulkAttendanceLineDto> Lines { get; set; } = new();
}
