using AutoMapper;
using SchoolManagement.Application.Attendance.Dtos;
using SchoolManagement.Application.Courses.Dtos;
using SchoolManagement.Application.PeriodAttendance.Dtos;
using SchoolManagement.Application.Students.Dtos;
using SchoolManagement.Application.Teachers.Dtos;
using SchoolManagement.Application.Timetables.Dtos;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>();
        CreateMap<CreateStudentDto, Student>();
        CreateMap<UpdateStudentDto, Student>();

        CreateMap<AttendanceRecord, AttendanceRecordDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student != null ? s.Student.Name : string.Empty));

        CreateMap<Subject, SubjectDto>();
        CreateMap<PeriodAttendanceRecord, PeriodAttendanceRecordDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student != null ? s.Student.Name : string.Empty))
            .ForMember(d => d.SubjectName, o => o.MapFrom(s => s.Subject != null ? s.Subject.Name : string.Empty));

        CreateMap<Teacher, TeacherDto>()
            .ForMember(d => d.Username, o => o.MapFrom(s => s.User != null ? s.User.Username : string.Empty));
        CreateMap<TeacherClassSubjectAllocation, TeacherAllocationDto>()
            .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher != null ? s.Teacher.FullName : string.Empty))
            .ForMember(d => d.SubjectName, o => o.MapFrom(s => s.Subject != null ? s.Subject.Name : string.Empty));

        CreateMap<Course, CourseDto>();
        CreateMap<CourseSection, CourseSectionDto>();
        CreateMap<CourseSubject, CourseSubjectDto>()
            .ForMember(d => d.SubjectName, o => o.MapFrom(s => s.Subject != null ? s.Subject.Name : string.Empty));

        CreateMap<TimetableEntry, TimetableEntryDto>()
            .ForMember(d => d.SubjectName, o => o.MapFrom(s => s.Subject != null ? s.Subject.Name : string.Empty))
            .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher != null ? s.Teacher.FullName : string.Empty));
    }
}
