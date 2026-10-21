using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Users;
using Application.Users.Dtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Ports;
using Moq;

namespace Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IPasswordHasher> _passwordHasher = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();

        private UserService CreateService() =>
            new(_userRepository.Object, _passwordHasher.Object, _unitOfWork.Object);

        [Fact]
        public async Task CreateAdmin_Ok()
        {
            _userRepository.Setup(r => r.ExistsByUserNameAsync("admin", It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _passwordHasher.Setup(h => h.Hash("secret")).Returns("hashed-secret");

            User? captured = null;
            _userRepository.Setup(r => r.Add(It.IsAny<User>()))
                .Callback<User>(u => captured = u);

            var service = CreateService();

            var result = await service.CreateAdminAsync(new CreateUserRequest { UserName = "admin", Password = "secret" });

            Assert.Equal(UserRole.Admin.ToString(), result.Role);
            Assert.NotNull(captured);
            Assert.Equal("hashed-secret", captured!.Password);
            Assert.Equal(UserRole.Admin, captured.Role);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateClient_Ok()
        {
            _userRepository.Setup(r => r.ExistsByUserNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _passwordHasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed");

            var service = CreateService();

            var result = await service.CreateClientAsync(new CreateUserRequest { UserName = "client", Password = "secret" });

            Assert.Equal(UserRole.Client.ToString(), result.Role);
        }

        [Fact]
        public async Task CreateUser_DuplicateName()
        {
            _userRepository.Setup(r => r.ExistsByUserNameAsync("admin", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = CreateService();

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreateAdminAsync(new CreateUserRequest { UserName = "admin", Password = "secret" }));

            _userRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("", "secret")]
        [InlineData("   ", "secret")]
        [InlineData("admin", "")]
        [InlineData("admin", null)]
        public async Task CreateUser_InvalidData(string? userName, string? password)
        {
            var service = CreateService();

            await Assert.ThrowsAsync<ValidationException>(() =>
                service.CreateAdminAsync(new CreateUserRequest
                {
                    UserName = userName ?? string.Empty,
                    Password = password ?? string.Empty
                }));
        }
    }
}
