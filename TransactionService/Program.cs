using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TransactionInfrastructure;

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

builder.Services.AddHttpContextAccessor();
//builder.Services.AddMapsterConfigration();

builder.Services.AddHttpClient("CurriculumClient", (handler) =>
{
    handler.BaseAddress = new Uri(builder.Configuration["CurriculumClient:baseUrl"]);
});

builder.Services.AddHttpClient("IdentityClient", (handler) =>
{
    handler.BaseAddress = new Uri(builder.Configuration["IdentityClient:baseUrl"]);
});

builder.Services.AddDbContext<StudentTrackerDbContext>(options =>
{
    options.UseSqlServer(new SqlConnectionStringBuilder
    {
        //DataSource = @"AHMED\SQLEXPRESS",
        DataSource = @".",
        InitialCatalog = "STStudentTracker",
        IntegratedSecurity = true
    }.ConnectionString);
});

builder.Services.AddMediatR(typeof(IMarkupAssemblyScanning));
//builder.Services.AddValidatorsFromAssembly(typeof(IMarkupAssemblyScanning).Assembly);

var app = builder.Build();

//db connection


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    builder.Services.BuildServiceProvider().GetRequiredService<StudentTrackerDbContext>().Database.EnsureCreated();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
