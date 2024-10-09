using Durak.Api.Contracts.Dtos;
using Durak.Api.Entities;

namespace Durak.Api.Mappers;

public static class RoomMapperExtensions
{
    public static RoomDto ToRoomDto(this Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            IsGameStarted = room.IsGameInProgress,
            PlayerCount = room.PlayerCount,
            RoomSetting = room.RoomSetting.ToRoomSettingDto(),
            RoomUsers = room.RoomUsers.Select(RoomUserMapperExtensions.ToRoomUserDto).ToList()
        };
    }
}
