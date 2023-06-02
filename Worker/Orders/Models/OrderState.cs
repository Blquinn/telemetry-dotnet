using Common.Contracts;

namespace Worker.Orders.Models;

public class OrderState
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public OrderStatus Status { get; set; }
}
