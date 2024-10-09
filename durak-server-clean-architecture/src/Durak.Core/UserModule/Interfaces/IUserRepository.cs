using Durak.Core.UserModule.Entities;

namespace Durak.Core.UserModule.Interfaces;

public interface IUserRepository
{
    Task AddUserAsync(User user);
}