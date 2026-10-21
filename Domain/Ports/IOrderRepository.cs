using Domain.Entities;

namespace Domain.Ports
{
    public interface IOrderRepository
    {
        Task<IReadOnlyList<Order>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
        void Add(Order order);
    }
}
