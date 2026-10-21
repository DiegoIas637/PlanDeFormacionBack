using Application.Common.Security;
using Application.Orders;
using Application.Orders.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlanDeFormacionBack.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize(Roles = "Client")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ITokenGenerator _tokenGenerator;

        public OrdersController(IOrderService orderService, ITokenGenerator tokenGenerator)
        {
            _orderService = orderService;
            _tokenGenerator = tokenGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDetailResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllAsync(cancellationToken);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var userId = _tokenGenerator.GetUserId(User);
            var response = await _orderService.CreateAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
        }
    }
}
