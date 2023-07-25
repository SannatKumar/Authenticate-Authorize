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
        public async Task<ActionResult<ServiceRespone<int>>> Register(UserRegisterDto request)
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
        public async Task<ActionResult<ServiceRespone<int>>> Login(UserLoginDto request)
        {
            var response = await _authRepo.Login(request.Username, request.Password);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            if(response.token is not null)
            {
                // Set the cookie
                Response.Cookies.Append("token", response.token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(7),
                    HttpOnly = true // The cookie can only be accessed through HTTP requests (not JavaScript)
                                    // Other options like domain, path, etc., can be set as needed
                });
            }

            return Ok(response);
        }

        [HttpPost("refesh-token")]
        public async Task<ActionResult<ServiceRespone<int>>> RefreshToken(string refreshToken)
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

        [HttpGet("me")]
        public async Task<ActionResult<AuthServiceRespone<string>>> GetMe()
        {
            //var refreshToken = Request.Cookies["refreshToken"];
            var response = await _authRepo.GetMe();
            

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }



    }
}
