using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Azure;
using System.Security.Cryptography;
using ServiceButtonBackend.Services.UserService;
using ServiceButtonBackend.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceButtonBackend.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ServiceButtonBackend.Data
{
    public class AuthRepository : IAuthRepository
    {
        //For Db Context
        private readonly DataContext _context;
        //For IConfiguration
        private readonly IConfiguration _configuration;
        //For Context Accessor
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthRepository(DataContext context, IConfiguration configuration, IUserService userService, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
            _contextAccessor = contextAccessor;
        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            //check if user exist
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));

            //check the user is not null
            if (user is null) 
            {
                response.Success = false;
                response.Message = "User not Found.";
                return response;
            }
            //check the password is verified
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong Password.";
                return response;
            }
            //Set the user data to response.Data
            response.Data = CreateToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);
            var myToken = SetRefreshToken(refreshToken);
            

            return response;
        }

        //Register User 
        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var response = new ServiceResponse<int>();

            if(await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = "User already Exists.";
                return response;
            }
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;


            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            response.Data = user.Id;
            return response;
        }

        //Check If user Already Exists
        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        //Create Password Hash and Salt
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }


        //Verify Password Hash
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }


        //Create Token
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if(appSettingsToken is null)
            {
                throw new Exception("AppSettings Token is null!");
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettingsToken));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }


        //Refresh Token
        private async Task<RefreshToken> GenerateRefreshToken(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            UserRefreshTokenDto dbRefreshToken = new UserRefreshTokenDto();
            dbRefreshToken.UserId = userId;
            dbRefreshToken.RefreshToken = refreshToken.Token;
            dbRefreshToken.TokenExpiresAt = refreshToken.Expires;
            dbRefreshToken.TokenCreated = refreshToken.Created;

            _context.UserRefreshToken.Add(dbRefreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        ////Set Refresh Token
        private string SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
            };
            //Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            //user.RefreshToken = newRefreshToken.Token;
            //user.TokenCreated = newRefreshToken.Created;
            //user.TokenExpires = newRefreshToken.Expires;

            return newRefreshToken.ToString()!;

        }


        public Task<ServiceResponse<string>> RefreshToken(string refreshToken, ClaimsPrincipal user)
        {
            //var user = string.Empty;

            //if(_contextAccessor.HttpContext != null)
            //{
            //    string loggedUser  = _contextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            //}






            ////if (!user.RefreshToken.Equals(refreshToken))
            ////{
            ////    return Unauthorized("Invalid Refresh Token");
            ////}
            ////else if (user.TokenExpires < DateTime.Now)
            ////{
            ////    return Unauthorized("Token expired");
            ////}

            ////string token = CreateToken(user);
            ////var newRefreshToken = GenerateRefreshToken();
            ////SetRefreshToken(newRefreshToken);

            //return Ok(token);
        }
        

    }
}
