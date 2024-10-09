using Durak.Api.Entities;

namespace Durak.Api.Repositories;
public interface IUserRepository
{
    Task<User> AddUserAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetUserAsync(long id, CancellationToken cancellationToken);
}