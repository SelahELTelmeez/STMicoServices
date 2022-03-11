using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityInfrastructure.Mapping;
using IdentityInfrastructure.Services;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; }); builder.Services.AddEndpointsApiExplorer();
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
    handler.DefaultRequestHeaders.Add("Username", builder.Configuration["SMSSettings:Username"]);
    handler.DefaultRequestHeaders.Add("Password", builder.Configuration["SMSSettings:Password"]);
    handler.DefaultRequestHeaders.Add("Msignature", builder.Configuration["SMSSettings:Msignature"]);
    handler.DefaultRequestHeaders.Add("Token", builder.Configuration["SMSSettings:Token"]);
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
