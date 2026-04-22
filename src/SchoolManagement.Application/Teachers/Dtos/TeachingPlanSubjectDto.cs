namespace SchoolManagement.Application.Teachers.Dtos;

public class TeachingPlanSubjectDto
{
    public int AllocationId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public bool IsClassTeacher { get; set; }
    public bool IsActive { get; set; }
    public IReadOnlyList<WeeklyPeriodSlotDto> WeeklyPeriods { get; set; } = Array.Empty<WeeklyPeriodSlotDto>();
}
