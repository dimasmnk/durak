using Durak.Server.API.Grains;
using Durak.Server.API.Models;
using Durak.Server.API.Services.Interfaces;

namespace Durak.Server.API.Services;

public class RoomInfoService(IGrainFactory grainFactory) : IRoomInfoService
{
    private readonly IGrainFactory _grainFactory = grainFactory;

    public async ValueTask<RoomState> GetRoomStateAsync(string roomKey)
    {
        if (string.IsNullOrEmpty(roomKey)) return null!;
        var room = _grainFactory.GetGrain<IRoomGrain>(roomKey);
        return await room.GetRoomStateAsync();
    }

    public async ValueTask<GameState> GetGameStateAsync(string roomKey, long playerId)
    {
        var room = _grainFactory.GetGrain<IRoomGrain>(roomKey);
        return await room.GetGameStateAsync(playerId);
    }
}
