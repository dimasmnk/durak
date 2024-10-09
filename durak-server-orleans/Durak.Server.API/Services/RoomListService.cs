using Durak.Server.API.Hubs.RoomListEvents;
using Durak.Server.API.Models;
using Durak.Server.API.Services.Interfaces;
using System.Collections.Concurrent;

namespace Durak.Server.API.Services;

public class RoomListService(IRoomListEventService roomListEventService, IRoomTokenService roomService) : IRoomListService
{
    private static readonly ConcurrentDictionary<string, Room> _rooms = [];
    private readonly IRoomTokenService _roomService = roomService;
    private readonly IRoomListEventService _roomListEventService = roomListEventService;

    public ICollection<Room> Rooms => _rooms.Values;

    public async Task AddRoomAsync(string roomId, RoomSettings roomSettings, CancellationToken cancellationToken)
    {
        if (_rooms.ContainsKey(roomId)) return;

        var room = new Room(roomId, roomSettings, 0);

        if (_rooms.TryAdd(roomId, room))
            await _roomListEventService.SendAsync(new AddRoomEvent(room.ConnectionId, roomSettings, 0), cancellationToken);
    }

    public async Task RemoveRoomAsync(string roomId, CancellationToken cancellationToken)
    {
        if (!_rooms.ContainsKey(roomId)) return;

        if (_rooms.TryRemove(roomId, out _))
            await _roomListEventService.SendAsync(new RemoveRoomEvent(roomId), cancellationToken);
    }

    public async Task UpdateRoomAsync(string roomId, int playerCount, CancellationToken cancellationToken = default)
    {
        if (!_rooms.TryGetValue(roomId, out var room)) return;

        room.PlayerCount = playerCount;

        await _roomListEventService.SendAsync(new UpdateRoomPlayerCountEvent(roomId, playerCount), cancellationToken);
    }

    public string GetRandomAvailableRoomConnectionId()
    {
        var availableRooms = _rooms.Values.Where(room => room.PlayerCount < 2);

        if (!availableRooms.Any())
        {
            var roomSettings = _roomService.CreateRoomSettings(Enums.Bet.Bet10, false);
            return _roomService.ConvertRoomSettingsToToken(roomSettings);
        }

        return availableRooms.First().ConnectionId;
    }
}
