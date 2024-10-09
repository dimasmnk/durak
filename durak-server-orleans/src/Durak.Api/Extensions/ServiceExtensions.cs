using Durak.Api.Services;

namespace Durak.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IRoomListEventService, RoomListEventService>();
    }
}
