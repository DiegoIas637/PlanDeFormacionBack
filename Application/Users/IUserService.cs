using Application.Users.Dtos;

namespace Application.Users
{
    public interface IUserService
    {
        Task<UserResponse> CreateAdminAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task<UserResponse> CreateClientAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    }
}
