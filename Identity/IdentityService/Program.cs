using IdentityInfrastructure;
using JWTGenerator.JWTModel;
using JWTGenerator.TokenHandler;
using Microsoft.OpenApi.Models;
using Serilog;
using SharedModule.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration)));


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Allow_Token_Validation_Policy",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                         .SetIsOriginAllowed((host) => true)
                         .WithExposedHeaders("Token-Expired")
                         .AllowAnyHeader();
                      });
});
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddJWTTokenHandlerExtension(new JWTConfiguration
{
    Audience = builder.Configuration["Jwt:Audience"],
    Issuer = builder.Configuration["Jwt:Issuer"],
    Key = builder.Configuration["Jwt:Key"],
    AccessTokenExpiration = TimeSpan.FromDays(int.Parse(builder.Configuration["Jwt:AccessTokenExpiration"])),
    ClearCliamTypeMap = true,
});
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
builder.Services.AddHttpClient("SMSClient", (handler) =>
{
    handler.BaseAddress = new Uri(builder.Configuration["SMSSettings:BaseUrl"]);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructureDIContainer(builder.Configuration);


var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("Allow_Token_Validation_Policy");

app.MapControllers();



app.Run();
