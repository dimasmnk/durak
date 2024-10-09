using Durak.Server.API.Enums;
using Durak.Server.API.Models;
using Durak.Server.API.Services.Interfaces;
using System.Text.Json;

namespace Durak.Server.API.Services;

public class RoomTokenService : IRoomTokenService
{
    public RoomSettings CreateRoomSettings(Bet bet, bool isPrivate)
    {
        var randomGuid = Guid.NewGuid();
        return new RoomSettings(randomGuid, bet, isPrivate);
    }

    public string ConvertRoomSettingsToToken(RoomSettings roomSettings)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(roomSettings);
        return Convert.ToBase64String(bytes);
    }

    public RoomSettings ConvertTokenToRoomSettings(string token)
    {
        var roomSettings = JsonSerializer.Deserialize<RoomSettings>(Convert.FromBase64String(token));

        return roomSettings is null
            ? throw new ArgumentException("Failed to deserialize room settings", nameof(token))
            : roomSettings;
    }
}
