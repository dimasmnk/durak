using Durak.Api.Entities;
using Durak.Api.Persistence;

namespace Durak.Api.Repositories;

public class PublicRoomRepository(IDurakDbContext context) : IPublicRoomRepository
{
    private readonly IDurakDbContext _context = context;

    public async Task AddPublicRoomAsync(PublicRoom publicRoom, CancellationToken cancellationToken)
    {
        await _context.PublicRooms.AddAsync(publicRoom, cancellationToken);
    }
}
