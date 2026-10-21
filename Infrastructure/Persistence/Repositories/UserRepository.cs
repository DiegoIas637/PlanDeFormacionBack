using Domain.Entities;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        }

        public Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return _context.Users.AnyAsync(u => u.UserName == userName, cancellationToken);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }
    }
}
