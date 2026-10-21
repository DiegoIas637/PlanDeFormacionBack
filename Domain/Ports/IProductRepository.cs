using Domain.Entities;

namespace Domain.Ports
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold, CancellationToken cancellationToken = default);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> TryDecrementStockAsync(Guid id, int quantity, CancellationToken cancellationToken = default);
        void Add(Product product);
        void Update(Product product);
        void Remove(Product product);
    }
}
