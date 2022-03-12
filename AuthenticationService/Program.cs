using FluentValidation;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityInfrastructure;
using IdentityInfrastructure.Mapping;
using IdentityInfrastructure.Services;
using JsonLocalizer;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddJWTTokenHandlerExtension(new JWTConfiguration
{
    Audience = builder.Configuration["Jwt:Audience"],
    Issuer = builder.Configuration["Jwt:Issuer"],
    Key = builder.Configuration["Jwt:Key"],
    AccessTokenExpiration = TimeSpan.FromDays(int.Parse(builder.Configuration["Jwt:AccessTokenExpiration"])),
    ClearCliamTypeMap = true,
});
builder.Services.AddJsonLocalizer(builder.Environment.WebRootPath, new CultureInfo("en-US"), new CultureInfo("ar-EG"));
builder.Services.AddEndpointsApiExplorer();
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
builder.Services.AddDbContext<STIdentityDbContext>(options =>
{
    options.UseSqlServer(new SqlConnectionStringBuilder
    {
        DataSource = @".",
        InitialCatalog = "STIdentity",
        IntegratedSecurity = true
    }.ConnectionString);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddMapsterConfigration();
builder.Services.AddHttpClient("SMSClient", (handler) =>
{
    handler.BaseAddress = new Uri(builder.Configuration["SMSSettings:BaseUrl"]);
});

builder.Services.AddMediatR(typeof(IMarkupAssemblyScanning));
builder.Services.AddValidatorsFromAssembly(typeof(IMarkupAssemblyScanning).Assembly);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    builder.Services.BuildServiceProvider().GetRequiredService<STIdentityDbContext>().Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
