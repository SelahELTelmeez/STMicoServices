using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ServicesGateway.Delegates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("ocelot.json");
});
builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<AcceptLanguageDelegatingHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.UseOcelot();

app.UseRouting();

app.Run();