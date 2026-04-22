using AutoMapper;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Courses.Dtos;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Courses;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courses;
    private readonly ISubjectRepository _subjects;
    private readonly IMapper _mapper;

    public CourseService(ICourseRepository courses, ISubjectRepository subjects, IMapper mapper)
    {
        _courses = courses;
        _subjects = subjects;
        _mapper = mapper;
    }

    public async Task<PagedResult<CourseDto>> GetPagedCoursesAsync(int page, int pageSize, string? search, bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var rows = await _courses.GetPagedCoursesAsync(page, pageSize, search, activeOnly, cancellationToken);
        return new PagedResult<CourseDto> { Total = rows.Total, Page = rows.Page, PageSize = rows.PageSize, Data = _mapper.Map<IReadOnlyList<CourseDto>>(rows.Data) };
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _courses.GetCourseByIdAsync(id, cancellationToken);
        return row == null ? null : _mapper.Map<CourseDto>(row);
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto, CancellationToken cancellationToken = default)
    {
        if (await _courses.GetCourseByNameAsync(dto.Name.Trim(), cancellationToken) != null)
            throw new InvalidOperationException("Course/Class name already exists.");

        var course = new Course { Name = dto.Name.Trim(), Code = string.IsNullOrWhiteSpace(dto.Code) ? null : dto.Code.Trim(), IsActive = dto.IsActive };
        await _courses.AddCourseAsync(course, cancellationToken);
        var created = await _courses.GetCourseByIdAsync(course.Id, cancellationToken) ?? course;
        return _mapper.Map<CourseDto>(created);
    }

    public async Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto dto, CancellationToken cancellationToken = default)
    {
        var course = await _courses.GetCourseByIdAsync(id, cancellationToken);
        if (course == null) return null;

        var dup = await _courses.GetCourseByNameAsync(dto.Name.Trim(), cancellationToken);
        if (dup != null && dup.Id != id)
            throw new InvalidOperationException("Course/Class name already exists.");

        course.Name = dto.Name.Trim();
        course.Code = string.IsNullOrWhiteSpace(dto.Code) ? null : dto.Code.Trim();
        course.IsActive = dto.IsActive;
        await _courses.UpdateCourseAsync(course, cancellationToken);
        return _mapper.Map<CourseDto>(course);
    }

    public Task<bool> DeleteCourseAsync(int id, CancellationToken cancellationToken = default) => _courses.DeleteCourseAsync(id, cancellationToken);

    public async Task<IReadOnlyList<CourseSectionDto>> GetSectionsAsync(int courseId, bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var rows = await _courses.GetSectionsAsync(courseId, activeOnly, cancellationToken);
        return _mapper.Map<IReadOnlyList<CourseSectionDto>>(rows);
    }

    public async Task<CourseSectionDto> CreateSectionAsync(int courseId, CreateCourseSectionDto dto, CancellationToken cancellationToken = default)
    {
        if (await _courses.GetCourseByIdAsync(courseId, cancellationToken) == null)
            throw new InvalidOperationException("Course/Class not found.");

        var sectionName = dto.Section.Trim();
        if (await _courses.ExistsSectionAsync(courseId, sectionName, null, cancellationToken))
            throw new InvalidOperationException("Section already exists in this course/class.");

        var row = new CourseSection { CourseId = courseId, Section = sectionName, IsActive = dto.IsActive };
        await _courses.AddSectionAsync(row, cancellationToken);
        return _mapper.Map<CourseSectionDto>(row);
    }

    public async Task<CourseSectionDto?> UpdateSectionAsync(int sectionId, UpdateCourseSectionDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _courses.GetSectionByIdAsync(sectionId, cancellationToken);
        if (row == null) return null;

        var sectionName = dto.Section.Trim();
        if (await _courses.ExistsSectionAsync(row.CourseId, sectionName, sectionId, cancellationToken))
            throw new InvalidOperationException("Section already exists in this course/class.");

        row.Section = sectionName;
        row.IsActive = dto.IsActive;
        await _courses.UpdateSectionAsync(row, cancellationToken);
        return _mapper.Map<CourseSectionDto>(row);
    }

    public Task<bool> DeleteSectionAsync(int sectionId, CancellationToken cancellationToken = default) => _courses.DeleteSectionAsync(sectionId, cancellationToken);

    public async Task<IReadOnlyList<CourseSubjectDto>> GetCourseSubjectsAsync(int courseId, bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var rows = await _courses.GetSubjectsAsync(courseId, activeOnly, cancellationToken);
        return _mapper.Map<IReadOnlyList<CourseSubjectDto>>(rows);
    }

    public async Task<CourseSubjectDto> CreateCourseSubjectAsync(int courseId, CreateCourseSubjectDto dto, CancellationToken cancellationToken = default)
    {
        if (await _courses.GetCourseByIdAsync(courseId, cancellationToken) == null)
            throw new InvalidOperationException("Course/Class not found.");

        if (await _subjects.GetByIdAsync(dto.SubjectId, cancellationToken) == null)
            throw new InvalidOperationException("Subject not found.");

        if (await _courses.ExistsCourseSubjectAsync(courseId, dto.SubjectId, null, cancellationToken))
            throw new InvalidOperationException("Subject already mapped to this course/class.");

        var row = new CourseSubject { CourseId = courseId, SubjectId = dto.SubjectId, IsActive = dto.IsActive };
        await _courses.AddCourseSubjectAsync(row, cancellationToken);
        var created = await _courses.GetCourseSubjectByIdAsync(row.Id, cancellationToken) ?? row;
        return _mapper.Map<CourseSubjectDto>(created);
    }

    public async Task<CourseSubjectDto?> UpdateCourseSubjectAsync(int id, UpdateCourseSubjectDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _courses.GetCourseSubjectByIdAsync(id, cancellationToken);
        if (row == null) return null;

        if (await _subjects.GetByIdAsync(dto.SubjectId, cancellationToken) == null)
            throw new InvalidOperationException("Subject not found.");

        if (await _courses.ExistsCourseSubjectAsync(row.CourseId, dto.SubjectId, id, cancellationToken))
            throw new InvalidOperationException("Subject already mapped to this course/class.");

        row.SubjectId = dto.SubjectId;
        row.IsActive = dto.IsActive;
        await _courses.UpdateCourseSubjectAsync(row, cancellationToken);
        var updated = await _courses.GetCourseSubjectByIdAsync(id, cancellationToken) ?? row;
        return _mapper.Map<CourseSubjectDto>(updated);
    }

    public Task<bool> DeleteCourseSubjectAsync(int id, CancellationToken cancellationToken = default) => _courses.DeleteCourseSubjectAsync(id, cancellationToken);
}
