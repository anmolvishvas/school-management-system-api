using AutoMapper;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Students.Dtos;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Students;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _students;
    private readonly IMapper _mapper;

    public StudentService(IStudentRepository students, IMapper mapper)
    {
        _students = students;
        _mapper = mapper;
    }

    public async Task<PagedResult<StudentDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string sortBy,
        string order,
        string? className,
        string? section,
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var pageResult = await _students.GetPagedAsync(
            page,
            pageSize,
            search,
            sortBy,
            order,
            className,
            section,
            activeOnly,
            cancellationToken);

        return new PagedResult<StudentDto>
        {
            Total = pageResult.Total,
            Page = pageResult.Page,
            PageSize = pageResult.PageSize,
            Data = _mapper.Map<IReadOnlyList<StudentDto>>(pageResult.Data)
        };
    }

    public async Task<StudentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _students.GetByIdAsync(id, cancellationToken);
        return entity == null ? null : _mapper.Map<StudentDto>(entity);
    }

    public async Task<StudentDto> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Student>(dto);
        await _students.AddAsync(entity, cancellationToken);
        return _mapper.Map<StudentDto>(entity);
    }

    public async Task<StudentDto?> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _students.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return null;

        _mapper.Map(dto, entity);
        await _students.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<StudentDto>(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _students.DeleteAsync(id, cancellationToken);
    }

    public async Task<StudentStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var total = await _students.CountAsync(cancellationToken);
        var byClass = await _students.CountByClassAsync(cancellationToken);
        var bySection = await _students.CountBySectionAsync(cancellationToken);

        return new StudentStatsDto
        {
            TotalStudents = total,
            ByClass = byClass,
            BySection = bySection
        };
    }
}
