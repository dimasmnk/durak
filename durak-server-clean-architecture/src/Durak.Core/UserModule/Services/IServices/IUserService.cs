using Durak.Core.Common.Interfaces;
using Durak.Core.UserModule.Entities;

namespace Durak.Core.UserModule.Services.IServices;

public interface IUserService : IService
{
    Task<User> CreateUser(string firstName);
}