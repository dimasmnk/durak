using Durak.Server.API.Enums;
using Durak.Server.API.Models;

namespace Durak.Server.API.Services.Interfaces;
public interface IRoomTokenService
{
    string ConvertRoomSettingsToToken(RoomSettings roomSettings);
    RoomSettings ConvertTokenToRoomSettings(string token);
    RoomSettings CreateRoomSettings(Bet bet, bool isPrivate);
}