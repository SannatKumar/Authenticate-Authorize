using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceButtonBackend.Dtos.User;
using ServiceButtonBackend.Services.DashboardService;

namespace ServiceButtonBackend.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService) 
        {
            _dashboardService = dashboardService;

        }

        [HttpPost("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            var response = await _dashboardService.GetDashboardData();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
