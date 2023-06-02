using Common.Contracts;
using MassTransit;
using WebApp;
using Worker.Orders.Models;

namespace Worker.Orders.Services;

class OrdersConsumer : IConsumer<OrderDto>
{
    private readonly WorkerDbContext _db;
    private readonly ILogger _logger;

    public OrdersConsumer(WorkerDbContext db, ILogger<OrdersConsumer> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderDto> context)
    {
        _logger.LogInformation("Received order message");

        var orderState = new OrderState()
        {
            OrderId = context.Message.Id,
            Status = OrderStatus.Processing,
        };

        await _db.AddAsync(orderState);
        await _db.SaveChangesAsync();
    
        _logger.LogInformation("Waiting 5 seconds");
        await Task.Delay(TimeSpan.FromSeconds(5));
        orderState.Status = OrderStatus.Complete;

        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Marked order complete");
    }
}
