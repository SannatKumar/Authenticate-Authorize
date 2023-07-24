using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using ServiceButtonBackend.Dtos.User;
using ServiceButtonBackend.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace ServiceButtonBackend.Data
{
    public class AuthRepository : IAuthRepository
    {
        //For Db Context
        private readonly DataContext _context;

        //For IConfiguration
        private readonly IConfiguration _configuration;

        ////For Context Accessor
        private readonly IHttpContextAccessor _contextAccessor;

        //For User Service
        private readonly IUserService _userService;

        //Create a Constructor for the DataContext, Configuration and HttpContext Accessor And User Service
        public AuthRepository(DataContext context, IConfiguration configuration, IHttpContextAccessor contextAccessor, IUserService userService)
        {
            _context = context;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _userService = userService;
        }

        //Function to login the User
        public async Task<AuthServiceRespone<string>> Login(string username, string password)
        {
            //var response = new ServiceRespone<string>();
            var authResponse = new AuthServiceRespone<string>();
            //check if user exist
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));

            //check the user is not null
            if (user is null) 
            {
                authResponse.Success = false;
                authResponse.Message = "User not Found.";
                return authResponse;
            }
            //check the password is verified
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                authResponse.Success = false;
                authResponse.Message = "Wrong Password.";
                return authResponse;
            }

            //Set the user data to response.Data
            //response.Data = CreateToken(user);
            authResponse.token = CreateToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);
            var myToken = SetRefreshToken(await refreshToken);

            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == user.Id);


            LoginResponseDto loginResponse = new LoginResponseDto();
            if(userDetails is not null)
            {

                loginResponse.Id = userDetails.Id;
                loginResponse.Email = userDetails.Email;
                loginResponse.User = user.Username;
                loginResponse.Locale = userDetails.Locale;
            }

            authResponse.UserDetail = loginResponse;

            //Ge The Permissions for the user details.
            List<UserPermission> dbPagePermissions = await _context.v_user_permission.Where(u => u.UserId == user.Id).ToListAsync();

            authResponse.UserPermisssion = dbPagePermissions;

            //Return Response
            return authResponse;
        }

        //Register User 
        public async Task<ServiceRespone<int>> Register(User user, string password)
        {
            //Create a new Response Object
            var response = new ServiceRespone<int>();

            //Check if user already Exists and return the Messages
            if(await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = "User already Exists.";
                return response;
            }
            //Generate Password hash and Password Salt
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            //Assign the newly created Passwword Hash and Salt to user
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            //Add The user to the context and Save Changes
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            //Set Response Data to 1 or True adn Return Response
            response.Data = 1;
            return response;
        }

        //Check If user Already Exists
        public async Task<bool> UserExists(string username)
        {
            //Check from User Table if the user already exists
            if(await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        //Create Password Hash and Salt
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //Creae a PasswordHash and Password Salt using System.Security.Cryptography
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }


        //Verify Password Hash
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            //Verify Password Hash using the hmac computed Hash
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }


        //Create Token
        private string CreateToken(User user)
        {
            //Use Claims to create a token with user ID, User Name and Role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            //Get the Token from appsettings using the configuration  
            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            //Check if Appsettings is null
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
            //Generate new refresh Token
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            //Prepare the token model and assign the new generated token and its properties
            UserRefreshTokenDto dbRefreshToken = new UserRefreshTokenDto();
            dbRefreshToken.UserId = userId;
            dbRefreshToken.RefreshToken = refreshToken.Token;
            dbRefreshToken.TokenExpiresAt = refreshToken.Expires;
            dbRefreshToken.TokenCreated = refreshToken.Created;

            //Get the existing token if exist  
            var existingRefreshToken = await _context.UserRefreshToken.FirstOrDefaultAsync(u => u.UserId == userId);
            //if exist update the new values else add a new row to the table
            if (existingRefreshToken != null) 
            {
                existingRefreshToken.RefreshToken = refreshToken.Token;
                existingRefreshToken.TokenExpiresAt = refreshToken.Expires;
                existingRefreshToken.TokenCreated = refreshToken.Created;
            }
            else
            {
                _context.UserRefreshToken.Add(dbRefreshToken);
            }

            //Finally Save the Changes 
            await _context.SaveChangesAsync();

            //Return Token to the calling function
            return refreshToken;
        }

        ////Set Refresh Token
        private string SetRefreshToken(RefreshToken newRefreshToken)
        {
            // Create a new variable with the Cookie Options 
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
            };

            //Access The httpcontext so that the response can be used outsisde Controllers. 
            var httpContext = _contextAccessor.HttpContext;
            if (httpContext == null)
            {
                // Handle the case when HttpContext is not available (e.g., during testing).
                throw new InvalidOperationException("HttpContext is not available.");
            }
            //Append The cookies to the response of this http Context
            httpContext.Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            //user.RefreshToken = newRefreshToken.Token;
            //user.TokenCreated = newRefreshToken.Created;
            //user.TokenExpires = newRefreshToken.Expires;


            return newRefreshToken.ToString()!;

        }
        //Get New Access Token and Refresh The Refresh Token 
        public async Task<ServiceRespone<string>> RefreshToken(string refreshToken)
        {
            //New Response Object
            var response = new ServiceRespone<string>();

            //Get The User ID
            var userId = _userService.GetUserId();

            //Get User Here
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            //Get The Old Refresh Token
            var oldRefreshToken = refreshToken;

            //Get the existing token if exist  
            var existingRefreshToken = await _context.UserRefreshToken.FirstOrDefaultAsync(u => u.UserId == userId);
            //check for Null
            if(existingRefreshToken is null)
            {
                response.Success = false;
                response.Message = "Refresh Token Does Not Exist for this User.";
                return response;
            }

            if (!existingRefreshToken.RefreshToken.Equals(refreshToken))
            {
                response.Success = false;
                response.Message = "Refresh Token is not Matching.";
                return response;
            }
            else if (existingRefreshToken.TokenExpiresAt < DateTime.Now)
            {
                response.Success = false;
                response.Message = "Refresh Token Has Expired.";
                return response;
            }

            //Set the user data to response.Data
            if(user is not null)
            {
                response.Data = CreateToken(user);
                var newRefreshToken = GenerateRefreshToken(user.Id);
                var myToken = SetRefreshToken(await newRefreshToken);
            }

            
            return response;
        }

        public async Task<AuthServiceRespone<string>> GetMe()
        {
            //Auth Response
            var authResponse = new AuthServiceRespone<string>();

            //Get The User ID
            var userId = _userService.GetUserId();

            //Get User From Database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            //check the user is not null
            if (user is null)
            {
                authResponse.Success = false;
                authResponse.Message = "User not Found.";
                return authResponse;
            }

            ////check the password is verified
            //if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            //{
            //    authResponse.Success = false;
            //    authResponse.Message = "Wrong Password.";
            //    return authResponse;
            //}

            //Set the user data to response.Data
            //response.Data = CreateToken(user);
            authResponse.token = CreateToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);
            var myToken = SetRefreshToken(await refreshToken);

            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == user.Id);


            LoginResponseDto loginResponse = new LoginResponseDto();
            if (userDetails is not null)
            {

                loginResponse.Id = userDetails.Id;
                loginResponse.Email = userDetails.Email;
                loginResponse.User = user.Username;
                loginResponse.Locale = userDetails.Locale;
            }

            authResponse.UserDetail = loginResponse;

            //Ge The Permissions for the user details.
            List<UserPermission> dbPagePermissions = await _context.v_user_permission.Where(u => u.UserId == user.Id).ToListAsync();

            authResponse.UserPermisssion = dbPagePermissions;

            return authResponse;

        }




    }
}
