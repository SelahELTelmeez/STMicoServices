using CurriculumEntites.Entities;
using CurriculumInfrastructure;
using FluentValidation;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Selaheltelmeez API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field (Add Space between Bearer and JWT Token)",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      Array.Empty<string>()
    }
  });
});

builder.Services.AddJWTTokenHandlerExtension(new JWTConfiguration
{
    Audience = builder.Configuration["Jwt:Audience"],
    Issuer = builder.Configuration["Jwt:Issuer"],
    Key = builder.Configuration["Jwt:Key"],
    AccessTokenExpiration = TimeSpan.FromDays(int.Parse(builder.Configuration["Jwt:AccessTokenExpiration"])),
    ClearCliamTypeMap = true,
});

builder.Services.AddDbContext<CurriculumDbContext>(options =>
{
    options.UseSqlServer(new SqlConnectionStringBuilder
    {
        //DataSource = @"AHMED\SQLEXPRESS",
        DataSource = @".",
        InitialCatalog = "STCurriculum",
        IntegratedSecurity = true
    }.ConnectionString);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructureDIContainer();
builder.Services.AddMediatR(typeof(IMarkupAssemblyScanning));
builder.Services.AddValidatorsFromAssembly(typeof(IMarkupAssemblyScanning).Assembly);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    builder.Services.BuildServiceProvider().GetRequiredService<CurriculumDbContext>().Database.EnsureCreated();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
