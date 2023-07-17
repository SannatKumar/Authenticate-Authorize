using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace ServiceButtonBackend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

        }
        public string GetMyName()
        {
            var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }

            return result!;
        }

        //Get User Id
        public int GetUserId()
        {
            int result = 0;
            if (_httpContextAccessor.HttpContext != null)
            {
                var idResult = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if(idResult is not null)
                {
                    result = int.Parse(idResult);
                }
                else
                {
                    result = 0;
                }   
            }

            return result!;
        }
        
        //Get User from The Database
        public int GetUserDetail()
        {
            int result = 0;
            if (_httpContextAccessor.HttpContext != null)
            {
                var idResult = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (idResult is not null)
                {
                    result = int.Parse(idResult);
                }
                else
                {
                    result = 0;
                }
            }

            return result!;
        }
    }
}
