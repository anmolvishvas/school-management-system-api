using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Common.Interfaces;

public interface ICourseRepository
{
    Task<PagedResult<Course>> GetPagedCoursesAsync(int page, int pageSize, string? search, bool? activeOnly, CancellationToken cancellationToken = default);
    Task<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Course?> GetCourseByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddCourseAsync(Course course, CancellationToken cancellationToken = default);
    Task UpdateCourseAsync(Course course, CancellationToken cancellationToken = default);
    Task<bool> DeleteCourseAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CourseSection>> GetSectionsAsync(int courseId, bool? activeOnly, CancellationToken cancellationToken = default);
    Task<CourseSection?> GetSectionByIdAsync(int sectionId, CancellationToken cancellationToken = default);
    Task<bool> ExistsSectionAsync(int courseId, string section, int? exceptId, CancellationToken cancellationToken = default);
    Task AddSectionAsync(CourseSection section, CancellationToken cancellationToken = default);
    Task UpdateSectionAsync(CourseSection section, CancellationToken cancellationToken = default);
    Task<bool> DeleteSectionAsync(int sectionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CourseSubject>> GetSubjectsAsync(int courseId, bool? activeOnly, CancellationToken cancellationToken = default);
    Task<CourseSubject?> GetCourseSubjectByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsCourseSubjectAsync(int courseId, int subjectId, int? exceptId, CancellationToken cancellationToken = default);
    Task AddCourseSubjectAsync(CourseSubject row, CancellationToken cancellationToken = default);
    Task UpdateCourseSubjectAsync(CourseSubject row, CancellationToken cancellationToken = default);
    Task<bool> DeleteCourseSubjectAsync(int id, CancellationToken cancellationToken = default);
}
