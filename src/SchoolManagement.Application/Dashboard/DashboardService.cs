using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;

namespace SchoolManagement.Application.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IStudentRepository _students;
    private readonly IAttendanceRepository _attendance;

    public DashboardService(IStudentRepository students, IAttendanceRepository attendance)
    {
        _students = students;
        _attendance = attendance;
    }

    public async Task<DashboardKpiDto> GetKpisAsync(CancellationToken cancellationToken = default)
    {
        var total = await _students.CountAsync(cancellationToken);

        var to = DateOnly.FromDateTime(DateTime.UtcNow);
        var from = to.AddDays(-30);
        var attendanceRate = await _attendance.GetAttendanceRatePercentAsync(from, to, null, null, cancellationToken);

        return new DashboardKpiDto
        {
            TotalStudents = total,
            AttendanceRatePercent = attendanceRate,
            FeeCollectionThisMonth = null
        };
    }
}
