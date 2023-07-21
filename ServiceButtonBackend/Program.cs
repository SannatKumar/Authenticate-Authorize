global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using ServiceButtonBackend.Models;
global using ServiceButtonBackend.Services.CharacterService;
global using ServiceButtonBackend.Dtos.Character;
global using ServiceButtonBackend.Data;
global using System.Text;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using ServiceButtonBackend.Services.UserService;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using ServiceButtonBackend.Services.DashboardService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://192.168.0.77:8080", "https://192.168.0.77:8080").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
        });
});

// Add services to the container.
//Register the Db Context
builder.Services.AddDbContext<DataContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Register the controllers
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = """Standard Authorization header using the bearer Scheme. Example: "bearer {token}" """,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey 

    });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

//Register The Auto Mapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);
//Register The Authentication service
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
//Register The service
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Cofigure CORS POLICY
app.UseCors();
//Add Authentication before Authorization
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
