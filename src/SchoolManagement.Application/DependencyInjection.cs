using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.Application.Attendance;
using SchoolManagement.Application.Auth;
using SchoolManagement.Application.Courses;
using SchoolManagement.Application.Dashboard;
using SchoolManagement.Application.PeriodAttendance;
using SchoolManagement.Application.Students;
using SchoolManagement.Application.Teachers;
using SchoolManagement.Application.Timetables;

namespace SchoolManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IPeriodAttendanceService, PeriodAttendanceService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<ITimetableService, TimetableService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
