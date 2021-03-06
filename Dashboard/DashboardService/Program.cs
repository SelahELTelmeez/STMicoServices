using DashboardInfrastructure;
using Flaminco.JWTHandler.TokenHandler;
using Microsoft.OpenApi.Models;
using Serilog;
using SharedModule.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration)));


builder.Services.AddHttpContextAccessor();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
builder.Services.AddInfrastructureDIContainer(builder.Configuration);

builder.Services.AddJWTTokenHandlerExtension(options =>
{
    options.Audience = builder.Configuration["Jwt:Audience"];
    options.Issuer = builder.Configuration["Jwt:Issuer"];
    options.Key = builder.Configuration["Jwt:Key"];
    options.AccessTokenExpiration = TimeSpan.FromDays(int.Parse(builder.Configuration["Jwt:AccessTokenExpiration"]));
    options.ClearCliamTypeMap = true;
    options.RequireHttpsMetadata = false;
    options.SaveTokenInAuthProperties = false;
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<ErrorHandlerMiddleware>();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
