namespace Common.Contracts;

public class OrderStateDto
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public OrderStatus Status { get; set; }
}