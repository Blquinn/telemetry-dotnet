using AutoMapper;
using Common;
using Common.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using WebApp.Orders.Models;
using OrderDto = WebApp.Orders.Models.OrderDto;

namespace WebApp.Orders.Controllers;

[Route("/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ISendEndpointProvider _endpointProvider;
    private readonly HttpClient _httpClient;

    public OrderController(AppDbContext db, IMapper mapper, ISendEndpointProvider endpointProvider, HttpClient httpClient, ILogger<OrderController> logger)
    {
        _db = db;
        _mapper = mapper;
        _endpointProvider = endpointProvider;
        _httpClient = httpClient; _logger = logger; }

    [HttpPost]
    public async Task<OrderDto> CreateOrder([FromBody] OrderCreateDto dto)
    {
        var order = _mapper.Map<Order>(dto);
        await _db.AddAsync(order);
        await _db.SaveChangesAsync();
    
        // Send message to worker service
        var endpoint = await _endpointProvider.GetSendEndpoint(Endpoints.Orders);
        await endpoint.Send(_mapper.Map<Common.Contracts.OrderDto>(order));
        
        // Poll for order status
        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            _logger.LogInformation("Checking order status for order {Id}", order.Id);
            
            var res = await _httpClient.GetAsync($"https://localhost:7215/orders/{order.Id}");
            var state = await res.Content.ReadFromJsonAsync<OrderStateDto>();
            if (state == null)
            {
                throw new Exception("Failed to parse order");
            }
            
            if (state.Status == OrderStatus.Complete)
            {
                return _mapper.Map<OrderDto>(order);
            }
        } 
        
        throw new Exception("Order did not get processed in time.");
    }
}
