using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _db;

    public CourseRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<Course>> GetPagedCoursesAsync(int page, int pageSize, string? search, bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.Courses.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.Name.Contains(term) || (x.Code != null && x.Code.Contains(term)));
        }
        if (activeOnly == true) query = query.Where(x => x.IsActive);

        var total = await query.CountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.Name).Skip(Math.Max(0, page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<Course> { Total = total, Page = page, PageSize = pageSize, Data = data };
    }

    public Task<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Course?> GetCourseByNameAsync(string name, CancellationToken cancellationToken = default)
        => _db.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public async Task AddCourseAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _db.Courses.AddAsync(course, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCourseAsync(Course course, CancellationToken cancellationToken = default)
    {
        _db.Courses.Update(course);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteCourseAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row == null) return false;
        _db.Courses.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<CourseSection>> GetSectionsAsync(int courseId, bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.CourseSections.AsNoTracking().Where(x => x.CourseId == courseId);
        if (activeOnly == true) query = query.Where(x => x.IsActive);
        return await query.OrderBy(x => x.Section).ToListAsync(cancellationToken);
    }

    public Task<CourseSection?> GetSectionByIdAsync(int sectionId, CancellationToken cancellationToken = default)
        => _db.CourseSections.FirstOrDefaultAsync(x => x.Id == sectionId, cancellationToken);

    public Task<bool> ExistsSectionAsync(int courseId, string section, int? exceptId, CancellationToken cancellationToken = default)
    {
        var query = _db.CourseSections.AsNoTracking().Where(x => x.CourseId == courseId && x.Section == section);
        if (exceptId.HasValue) query = query.Where(x => x.Id != exceptId.Value);
        return query.AnyAsync(cancellationToken);
    }

    public async Task AddSectionAsync(CourseSection section, CancellationToken cancellationToken = default)
    {
        await _db.CourseSections.AddAsync(section, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSectionAsync(CourseSection section, CancellationToken cancellationToken = default)
    {
        _db.CourseSections.Update(section);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteSectionAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        var row = await _db.CourseSections.FirstOrDefaultAsync(x => x.Id == sectionId, cancellationToken);
        if (row == null) return false;
        _db.CourseSections.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<CourseSubject>> GetSubjectsAsync(int courseId, bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.CourseSubjects.AsNoTracking().Include(x => x.Subject).Where(x => x.CourseId == courseId);
        if (activeOnly == true) query = query.Where(x => x.IsActive);
        return await query.OrderBy(x => x.Subject!.Name).ToListAsync(cancellationToken);
    }

    public Task<CourseSubject?> GetCourseSubjectByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.CourseSubjects.AsNoTracking().Include(x => x.Subject).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsCourseSubjectAsync(int courseId, int subjectId, int? exceptId, CancellationToken cancellationToken = default)
    {
        var query = _db.CourseSubjects.AsNoTracking().Where(x => x.CourseId == courseId && x.SubjectId == subjectId);
        if (exceptId.HasValue) query = query.Where(x => x.Id != exceptId.Value);
        return query.AnyAsync(cancellationToken);
    }

    public async Task AddCourseSubjectAsync(CourseSubject row, CancellationToken cancellationToken = default)
    {
        await _db.CourseSubjects.AddAsync(row, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCourseSubjectAsync(CourseSubject row, CancellationToken cancellationToken = default)
    {
        _db.CourseSubjects.Update(row);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteCourseSubjectAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.CourseSubjects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row == null) return false;
        _db.CourseSubjects.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
