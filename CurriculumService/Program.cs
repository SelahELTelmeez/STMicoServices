using CurriculumEntites.Entities;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddJsonLocalizer(builder.Environment.WebRootPath, new CultureInfo("en-US"), new CultureInfo("ar-EG"));


builder.Services.AddDbContext<CurriculumDbContext>(options =>
{
    options.UseSqlServer(new SqlConnectionStringBuilder
    {
        DataSource = @".",
        InitialCatalog = "CurriculumDatabase",
        IntegratedSecurity = true
    }.ConnectionString);
});

builder.Services.AddJWTTokenHandlerExtension(new JWTConfiguration
{
    Audience = builder.Configuration["Jwt:Audience"],
    Issuer = builder.Configuration["Jwt:Issuer"],
    Key = builder.Configuration["Jwt:Key"],
    AccessTokenExpiration = TimeSpan.FromDays(int.Parse(builder.Configuration["Jwt:AccessTokenExpiration"])), // if not specified the default is 14 days
    RefreshTokenExpiration = TimeSpan.FromDays(int.Parse(builder.Configuration["Jwt:RefreshTokenExpiration"])), // if not specified the default is 90 days
    ClearCliamTypeMap = true,
});

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
