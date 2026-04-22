namespace SchoolManagement.Application.PeriodAttendance.Dtos;

public class BulkMarkPeriodFromTimetableDto
{
    public DateOnly Date { get; set; }
    public int TimetableEntryId { get; set; }
    public List<PeriodAttendanceLineDto> Lines { get; set; } = new();
}
