namespace SchoolManagement.Application.Common.Models;

public class ObservedWeeklyPeriodSlot
{
    public int SubjectId { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public int HourNumber { get; set; }
}
