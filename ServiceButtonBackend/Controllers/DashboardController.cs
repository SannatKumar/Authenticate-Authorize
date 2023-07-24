using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceButtonBackend.Dtos.User;
using ServiceButtonBackend.Services.DashboardService;

namespace ServiceButtonBackend.Controllers
{
    //[Authorize]
    [Route("api/v1/")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService) 
        {
            _dashboardService = dashboardService;

        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<Dictionary<string, int>>> GetDashboardData()
        {
            var response = await _dashboardService.GetDashboardData();

            //if (!response.success)
            //{
            //    return badrequest(response);
            //}

            return Ok(response);
        }
    }
}
