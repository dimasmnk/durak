using Durak.Core.UserModule.Entities;
using Durak.Core.UserModule.Interfaces;
using Durak.Core.UserModule.Services.IServices;

namespace Durak.Core.UserModule.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User> CreateUser(string firstName)
    {
        var user = User.CreateUser(firstName);
        await userRepository.AddUserAsync(user);
        return user;
    }
}