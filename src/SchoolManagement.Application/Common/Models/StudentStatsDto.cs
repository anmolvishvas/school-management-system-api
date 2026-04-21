namespace SchoolManagement.Application.Common.Models;

public class StudentStatsDto
{
    public int TotalStudents { get; set; }
    public IReadOnlyList<LabelCountDto> ByClass { get; set; } = Array.Empty<LabelCountDto>();
    public IReadOnlyList<LabelCountDto> BySection { get; set; } = Array.Empty<LabelCountDto>();
}
