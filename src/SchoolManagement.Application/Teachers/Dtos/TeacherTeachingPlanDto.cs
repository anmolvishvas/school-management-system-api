namespace SchoolManagement.Application.Teachers.Dtos;

public class TeacherTeachingPlanDto
{
    public int TeacherId { get; set; }
    public int UserId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public IReadOnlyList<TeachingPlanClassSectionDto> ClassSections { get; set; } = Array.Empty<TeachingPlanClassSectionDto>();
}
