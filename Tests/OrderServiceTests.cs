using Application.Common.Exceptions;
using Application.Orders;
using Application.Orders.Dtos;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Ports;
using Moq;

namespace Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepository = new();
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly Mock<IMapper> _mapper = new();

        private OrderService CreateService()
        {
            _unitOfWork
                .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
                .Returns((Func<Task> operation, CancellationToken _) => operation());

            return new OrderService(
                _orderRepository.Object,
                _productRepository.Object,
                _userRepository.Object,
                _unitOfWork.Object,
                _mapper.Object);
        }

        [Fact]
        public async Task CreateOrder_Ok()
        {
            var userId = 1;
            var product = new Product { Id = Guid.NewGuid(), Name = "Laptop", Price = 100m, Stock = 10, UserId = userId };

            _userRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = userId, UserName = "client", Role = UserRole.Client });
            _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _productRepository.Setup(r => r.TryDecrementStockAsync(product.Id, 3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = CreateService();

            var result = await service.CreateAsync(new CreateOrderRequest
            {
                ProductId = product.Id,
                Quantity = 3
            }, userId);

            Assert.Equal(product.Id, result.ProductId);
            Assert.Equal(3, result.Quantity);
            Assert.Equal(100m, result.AppliedPrice);
            _orderRepository.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_NoStock()
        {
            var userId = 1;
            var product = new Product { Id = Guid.NewGuid(), Name = "Laptop", Price = 100m, Stock = 1, UserId = userId };

            _userRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = userId, UserName = "client", Role = UserRole.Client });
            _productRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _productRepository.Setup(r => r.TryDecrementStockAsync(product.Id, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var service = CreateService();

            await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(new CreateOrderRequest
            {
                ProductId = product.Id,
                Quantity = 5
            }, userId));

            _orderRepository.Verify(r => r.Add(It.IsAny<Order>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public async Task CreateOrder_InvalidQuantity(int quantity)
        {
            var service = CreateService();

            await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(new CreateOrderRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = quantity
            }, 1));
        }

        [Fact]
        public async Task CreateOrder_UserNotFound()
        {
            _userRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var service = CreateService();

            await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(new CreateOrderRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1
            }, 99));
        }

        [Fact]
        public async Task CreateOrder_ProductNotFound()
        {
            var userId = 1;
            _userRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User { Id = userId, UserName = "client", Role = UserRole.Client });
            _productRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var service = CreateService();

            await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(new CreateOrderRequest
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1
            }, userId));
        }
    }
}
