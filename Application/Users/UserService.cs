using Application.Common.Exceptions;
using Application.Common.Security;
using Application.Users.Dtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Ports;

namespace Application.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public Task<UserResponse> CreateAdminAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            return CreateAsync(request, UserRole.Admin, cancellationToken);
        }

        public Task<UserResponse> CreateClientAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            return CreateAsync(request, UserRole.Client, cancellationToken);
        }

        private async Task<UserResponse> CreateAsync(CreateUserRequest request, UserRole role, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                throw new ValidationException("El nombre de usuario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ValidationException("La contraseña es obligatoria.");
            }

            var userName = request.UserName.Trim();

            if (await _userRepository.ExistsByUserNameAsync(userName, cancellationToken))
            {
                throw new ConflictException($"El nombre de usuario '{userName}' ya está en uso.");
            }

            var user = new User
            {
                UserName = userName,
                Password = _passwordHasher.Hash(request.Password),
                Role = role
            };

            _userRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = user.Role.ToString()
            };
        }
    }
}
