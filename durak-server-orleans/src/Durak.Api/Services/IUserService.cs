using Durak.Api.Entities;

namespace Durak.Api.Services;
public interface IUserService
{
    Task<User> GetOrCreateUserAsync(long id, CancellationToken cancellationToken);
    Task<int> GetUserBalanceAsync(long userId, CancellationToken cancellationToken);
    Task UpdateUsernameAsync(long id, string username, CancellationToken cancellationToken);
    Task WithdrawDepositAsync(long userId, int amount, CancellationToken cancellationToken);
}