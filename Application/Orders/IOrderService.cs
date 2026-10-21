using Application.Orders.Dtos;

namespace Application.Orders
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateAsync(CreateOrderRequest request, int userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OrderDetailResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
