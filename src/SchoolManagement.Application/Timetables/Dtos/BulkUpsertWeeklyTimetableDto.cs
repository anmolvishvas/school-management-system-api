namespace SchoolManagement.Application.Timetables.Dtos;

public class BulkUpsertWeeklyTimetableDto
{
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public List<BulkWeeklyTimetableLineDto> Lines { get; set; } = new();
}
