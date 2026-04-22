namespace SchoolManagement.Application.Teachers.Dtos;

public class TeachingPlanClassSectionDto
{
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public IReadOnlyList<TeachingPlanSubjectDto> Subjects { get; set; } = Array.Empty<TeachingPlanSubjectDto>();
}
