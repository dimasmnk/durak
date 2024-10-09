using Durak.Core.Common.Interfaces;

namespace Durak.Core.AuthModule.Services.IServices;

public interface IAuthService : IService
{
    string Authenticate(string telegramToken);
}