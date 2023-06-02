using AutoMapper;
using Common.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp;
using Worker.Orders.Models;

namespace Worker.Orders.Controllers;

[Route("/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly WorkerDbContext _db;
    private readonly IMapper _mapper;

    public OrderController(WorkerDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet("{orderId}")]
    public async Task<OrderStateDto> GetOrderStatus(long orderId)
    {
        var orderState = await _db.OrderStates.Where(o => o.OrderId == orderId).FirstAsync();
        return _mapper.Map<OrderStateDto>(orderState);
    }
}
