using Durak.Api.Entities;
using Durak.Api.Persistence;
using Durak.Api.Repositories;

namespace Durak.Api.Services;

public class UserService(IUserRepository userRepository, IDurakDbContext db, ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDurakDbContext _db = db;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<User> GetOrCreateUserAsync(long id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(id, cancellationToken);

        if (user is not null)
            return user;

        user = new User
        {
            Id = id,
            Username = string.Empty,
            CoinCount = 100,
            WinCount = 0,
            LossCount = 0,
            DrawCount = 0,
            TotalCount = 0
        };

        await _userRepository.AddUserAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} has been created", id);

        return user;
    }

    public async Task UpdateUsernameAsync(long id, string username, CancellationToken cancellationToken)
    {
        var user = await GetOrCreateUserAsync(id, cancellationToken);
        user.Username = username;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} has updated their username to {Username}", id, username);
    }

    public async Task<int> GetUserBalanceAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await GetOrCreateUserAsync(userId, cancellationToken);
        return user?.CoinCount ?? 0;
    }

    public async Task WithdrawDepositAsync(long userId, int amount, CancellationToken cancellationToken)
    {
        var user = await GetOrCreateUserAsync(userId, cancellationToken);

        if (user.CoinCount < amount)
            throw new InvalidOperationException("User does not have enough coins");

        user.CoinCount -= amount;
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} has withdrawn {Amount} coins", userId, amount);
    }
}
