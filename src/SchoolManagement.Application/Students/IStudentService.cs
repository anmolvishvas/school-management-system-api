using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Students.Dtos;

namespace SchoolManagement.Application.Students;

public interface IStudentService
{
    Task<PagedResult<StudentDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string sortBy,
        string order,
        string? className,
        string? section,
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task<StudentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentDto> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
    Task<StudentDto?> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<StudentStatsDto> GetStatsAsync(CancellationToken cancellationToken = default);
}
