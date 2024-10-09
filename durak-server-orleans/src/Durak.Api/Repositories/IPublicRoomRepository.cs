using Durak.Api.Entities;

namespace Durak.Api.Repositories;
public interface IPublicRoomRepository
{
    Task AddPublicRoomAsync(PublicRoom publicRoom, CancellationToken cancellationToken);
}