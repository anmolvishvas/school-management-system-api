using AutoMapper;
using SchoolManagement.Application.Students.Dtos;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>();
        CreateMap<CreateStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>();
    }
}
