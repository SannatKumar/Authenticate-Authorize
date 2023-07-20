using Microsoft.AspNetCore.Mvc;

namespace ServiceButtonBackend.Services.DashboardService
{
    public interface IDashboardService
    {
        //Task is needed when async is used: ServiceResponse is from ServiceResponse Class to send messages
        Task<IActionResult> GetDashboardData();

    }
}
