﻿using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Azure;
using System.Security.Cryptography;
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
        ////For Context Accessor
        private readonly IHttpContextAccessor _contextAccessor;

        //Create a Constructor for the DataContext, Configuration and HttpContext Accessor
        public AuthRepository(DataContext context, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        //Function to login the User
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
            var myToken = SetRefreshToken(await refreshToken);
            
            //Return Response
            return response;
        }

        //Register User 
        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            //Create a new Response Object
            var response = new ServiceResponse<int>();

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

        public Task<ServiceResponse<string>> RefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        /*
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
        */




    }
}
