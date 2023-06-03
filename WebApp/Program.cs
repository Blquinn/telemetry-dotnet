using System.Net;
using Common;
using Common.Middleware;
using Common.Otel;
using MassTransit;
using Serilog;
using WebApp;
using WebApp.Orders.Models;

var builder = WebApplication.CreateBuilder(args);

var serviceName = "WebApp";

Logging.ConfigureLogger(builder, serviceName);

// Add services to the container.

builder.Services.AddCommonTelemetry(serviceName);
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAutoMapper(c =>
{
    c.CreateMap<OrderCreateDto, Order>();
    c.CreateMap<Order, OrderDto>();
    c.CreateMap<Order, Common.Contracts.OrderDto>();
});

var clientHandler = new HttpClientHandler() 
{ 
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};

using var httpClient = new HttpClient(clientHandler);

builder.Services.AddSingleton(httpClient);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseSerilogRequestLogging();
app.UseAuthorization();
app.MapControllers();

app.Use(TraceIdHeaderMiddleware.Handler);
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();

Log.CloseAndFlush();
