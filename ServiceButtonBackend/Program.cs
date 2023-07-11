global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using ServiceButtonBackend.Models;
global using ServiceButtonBackend.Services.CharacterService;
global using ServiceButtonBackend.Dtos.Character;
global using ServiceButtonBackend.Data;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
//Register the Db Context
builder.Services.AddDbContext<DataContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Register the controllers
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register The Auto Mapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);
//Register The Authentication service
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
//Register The service
builder.Services.AddScoped<ICharacterService, CharacterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
