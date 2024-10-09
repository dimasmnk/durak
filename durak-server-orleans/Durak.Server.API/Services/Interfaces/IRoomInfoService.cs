using Durak.Server.API.Models;

namespace Durak.Server.API.Services.Interfaces;
public interface IRoomInfoService
{
    ValueTask<GameState> GetGameStateAsync(string roomKey, long playerId);
    ValueTask<RoomState> GetRoomStateAsync(string roomKey);
}