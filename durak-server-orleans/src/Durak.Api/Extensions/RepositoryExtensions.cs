using Durak.Api.Repositories;

namespace Durak.Api.Extensions;

public static class RepositoryExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomUserRepository, RoomUserRepository>();
        services.AddScoped<IPublicRoomRepository, PublicRoomRepository>();
    }
}
