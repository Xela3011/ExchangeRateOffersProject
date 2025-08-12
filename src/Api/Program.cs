using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Infrastructure.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var api1Url = builder.Configuration["Api1:url"] ?? throw new InvalidOperationException("Api1 URL is missing");
var api2Url = builder.Configuration["Api2:url"] ?? throw new InvalidOperationException("Api2 URL is missing");
var api3Url = builder.Configuration["Api3:url"] ?? throw new InvalidOperationException("Api3 URL is missing");

builder.Services.AddHttpClient("api1", c => c.BaseAddress = new Uri(api1Url));
builder.Services.AddHttpClient("api2", c => c.BaseAddress = new Uri(api2Url));
builder.Services.AddHttpClient("api3", c => c.BaseAddress = new Uri(api3Url));

builder.Services.AddTransient<IExchangeRateProvider>(sp =>
    new JsonProvider1(sp.GetRequiredService<IHttpClientFactory>().CreateClient("api1"), "api1/convert"));

builder.Services.AddTransient<IExchangeRateProvider>(sp =>
    new XmlProvider(sp.GetRequiredService<IHttpClientFactory>().CreateClient("api2"), "api2/convert"));

builder.Services.AddTransient<IExchangeRateProvider>(sp =>
    new JsonProvider2(sp.GetRequiredService<IHttpClientFactory>().CreateClient("api3"), "api3/convert"));

builder.Services.AddTransient<IExchangeRateService, ExchangeRateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "api");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
