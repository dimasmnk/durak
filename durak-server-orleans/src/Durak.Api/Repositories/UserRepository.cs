using Durak.Api.Entities;
using Durak.Api.Persistence;

namespace Durak.Api.Repositories;

public class UserRepository(IDurakDbContext db) : IUserRepository
{
    private readonly IDurakDbContext _db = db;

    public async Task<User?> GetUserAsync(long id, CancellationToken cancellationToken)
    {
        return await _db.Users.FindAsync([id], cancellationToken);
    }

    public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken)
    {
        await _db.Users.AddAsync(user, cancellationToken);
        return user;
    }
}
