using Application.Auth.Dtos;
using Application.Common.Exceptions;
using Application.Common.Security;
using Domain.Ports;

namespace Application.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ValidationException("El nombre de usuario y la contraseña son obligatorios.");
            }

            var user = await _userRepository.GetByUserNameAsync(request.UserName.Trim(), cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.Password, user.Password))
            {
                throw new UnauthorizedException("Credenciales inválidas.");
            }

            var token = _tokenGenerator.GenerateToken(user);

            return new LoginResponse
            {
                AccessToken = token.Value,
                UserName = user.UserName,
                Role = user.Role.ToString(),
                ExpiresAt = token.ExpiresAt
            };
        }
    }
}
