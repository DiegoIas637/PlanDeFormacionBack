using Domain.Entities;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Order>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Product)
                .OrderByDescending(o => o.Id)
                .ToListAsync(cancellationToken);
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
        }
    }
}
