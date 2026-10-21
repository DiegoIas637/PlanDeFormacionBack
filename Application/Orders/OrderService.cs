using Application.Common.Exceptions;
using Application.Orders.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Ports;

namespace Application.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<OrderDetailResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAllWithDetailsAsync(cancellationToken);
            return _mapper.Map<IReadOnlyList<OrderDetailResponse>>(orders);
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, int userId, CancellationToken cancellationToken = default)
        {
            if (request.Quantity <= 0)
            {
                throw new ValidationException("La cantidad debe ser mayor que cero.");
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el usuario con id {userId}.");

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el producto con id {request.ProductId}.");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                UserId = user.Id,
                Quantity = request.Quantity,
                AppliedPrice = product.Price
            };

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var decremented = await _productRepository.TryDecrementStockAsync(product.Id, request.Quantity, cancellationToken);

                if (!decremented)
                {
                    throw new ConflictException($"No hay stock suficiente para el producto '{product.Name}'.");
                }

                _orderRepository.Add(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

            return new OrderResponse
            {
                Id = order.Id,
                ProductId = order.ProductId,
                UserId = order.UserId,
                Quantity = order.Quantity,
                AppliedPrice = order.AppliedPrice
            };
        }
    }
}
