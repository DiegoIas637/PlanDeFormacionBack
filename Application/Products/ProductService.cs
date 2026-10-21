using Application.Common.Exceptions;
using Application.Products.Dtos;
using Domain.Entities;
using Domain.Ports;

namespace Application.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);
            return products.Select(Map).ToList();
        }

        public async Task<ProductResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el producto con id {id}.");

            return Map(product);
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request, int ownerId, CancellationToken cancellationToken = default)
        {
            Validate(request.Name, request.Price, request.Stock);

            var owner = await _userRepository.GetByIdAsync(ownerId, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el usuario con id {ownerId}.");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Price = request.Price,
                Stock = request.Stock,
                UserId = owner.Id
            };

            _productRepository.Add(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(product);
        }

        public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
        {
            Validate(request.Name, request.Price, request.Stock);

            var product = await _productRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el producto con id {id}.");

            product.Name = request.Name.Trim();
            product.Price = request.Price;
            product.Stock = request.Stock;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(product);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el producto con id {id}.");

            _productRepository.Remove(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static void Validate(string name, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("El nombre del producto es obligatorio.");
            }

            if (price < 0)
            {
                throw new ValidationException("El precio del producto no puede ser negativo.");
            }

            if (stock < 0)
            {
                throw new ValidationException("El stock del producto no puede ser negativo.");
            }
        }

        private static ProductResponse Map(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                UserId = product.UserId
            };
        }
    }
}
