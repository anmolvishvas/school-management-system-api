using SchoolManagement.Application.Common.Models;

namespace SchoolManagement.Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardKpiDto> GetKpisAsync(CancellationToken cancellationToken = default);
}
