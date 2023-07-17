using Microsoft.AspNetCore.Mvc;
using ServiceButtonBackend.Dtos.User;

namespace ServiceButtonBackend.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("sign-up")]
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

        [HttpPost("sign-in")]
        public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto request)
        {
            var response = await _authRepo.Login(request.Username, request.Password);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("refesh-token")]
        public async Task<ActionResult<ServiceResponse<int>>> RefreshToken(string refreshToken)
        {
            //Get The Refresh Token from The Cookies
           // var refreshToken = Request.Cookies["refreshToken"];
            var response = await _authRepo.RefreshToken(refreshToken!);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
       


    }
}
