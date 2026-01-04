using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var orderId = await _orderService.CreateOrderAsync(
            request.Amount,
            request.Currency,
            cancellationToken);

        return Accepted(new { OrderId = orderId });
    }
}

public record CreateOrderRequest(decimal Amount, string Currency);