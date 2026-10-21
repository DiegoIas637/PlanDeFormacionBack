using Application.Products.Dtos;

namespace Application.Products
{
    public interface IProductService
    {
        Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ProductResponse> CreateAsync(CreateProductRequest request, int ownerId, CancellationToken cancellationToken = default);
        Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
