using Common;
using Common.Contracts;
using Common.Middleware;
using Common.Otel;
using MassTransit;
using WebApp;
using Worker.Orders.Models;
using Worker.Orders.Services;

var builder = WebApplication.CreateBuilder(args);

var serviceName = "Worker";

Logging.ConfigureLogger(builder, serviceName);

// Add services to the container.

builder.Services.AddCommonTelemetry(serviceName);
builder.Services.AddDbContext<WorkerDbContext>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(c =>
{
    c.CreateMap<OrderState, OrderStateDto>();
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrdersConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("orders", e =>
        {
            e.ConfigureConsumer<OrdersConsumer>(context);            
        });
    });
});

// builder.Services.AddHostedService<>()

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

app.UseAuthorization();
app.MapControllers();

app.Use(TraceIdHeaderMiddleware.Handler);
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();
