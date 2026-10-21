using Domain.Entities;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => p.Stock <= threshold)
                .OrderBy(p => p.Stock)
                .ToListAsync(cancellationToken);
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<bool> TryDecrementStockAsync(Guid id, int quantity, CancellationToken cancellationToken = default)
        {
            var affected = await _context.Products
                .Where(p => p.Id == id && p.Stock >= quantity)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(p => p.Stock, p => p.Stock - quantity),
                    cancellationToken);

            return affected == 1;
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Remove(Product product)
        {
            _context.Products.Remove(product);
        }
    }
}
