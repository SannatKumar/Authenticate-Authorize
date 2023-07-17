using Microsoft.AspNetCore.Mvc;
using ServiceButtonBackend.Dtos.User;

namespace ServiceButtonBackend.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
        {
            var response = await _authRepo.Register(
                new User { Username = request.Username }, request.Password
                );
            
            if(!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto request)
        {
            var response = await _authRepo.Login(request.Username, request.Password);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /*
        [HttpPost("refesh-token")]
        public async Task<ActionResult<ServiceResponse<int>>> RefreshToken()
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _authRepo.RefreshToken(refreshToken!, currentUser);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        */


    }
}
