using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;

namespace SchoolManagement.Application.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IStudentRepository _students;

    public DashboardService(IStudentRepository students)
    {
        _students = students;
    }

    public async Task<DashboardKpiDto> GetKpisAsync(CancellationToken cancellationToken = default)
    {
        var total = await _students.CountAsync(cancellationToken);

        return new DashboardKpiDto
        {
            TotalStudents = total,
            AttendanceRatePercent = null,
            FeeCollectionThisMonth = null
        };
    }
}
