using TourismHajj.ViewModels;

namespace TourismHajj.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync();
}
