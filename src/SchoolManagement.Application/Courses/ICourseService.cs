using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Courses.Dtos;

namespace SchoolManagement.Application.Courses;

public interface ICourseService
{
    Task<PagedResult<CourseDto>> GetPagedCoursesAsync(int page, int pageSize, string? search, bool? activeOnly, CancellationToken cancellationToken = default);
    Task<CourseDto?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, CancellationToken cancellationToken = default);
    Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCourseAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CourseSectionDto>> GetSectionsAsync(int courseId, bool? activeOnly, CancellationToken cancellationToken = default);
    Task<CourseSectionDto> CreateSectionAsync(int courseId, CreateCourseSectionDto dto, CancellationToken cancellationToken = default);
    Task<CourseSectionDto?> UpdateSectionAsync(int sectionId, UpdateCourseSectionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteSectionAsync(int sectionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CourseSubjectDto>> GetCourseSubjectsAsync(int courseId, bool? activeOnly, CancellationToken cancellationToken = default);
    Task<CourseSubjectDto> CreateCourseSubjectAsync(int courseId, CreateCourseSubjectDto dto, CancellationToken cancellationToken = default);
    Task<CourseSubjectDto?> UpdateCourseSubjectAsync(int id, UpdateCourseSubjectDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCourseSubjectAsync(int id, CancellationToken cancellationToken = default);
}
