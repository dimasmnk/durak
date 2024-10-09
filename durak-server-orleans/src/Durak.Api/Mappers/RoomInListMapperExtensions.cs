using Durak.Api.Contracts.Dtos;
using Durak.Api.Entities;

namespace Durak.Api.Mappers;

public static class RoomInListMapperExtensions
{
    public static RoomInListDto ToRoomInListDto(this Room room)
    {
        return new RoomInListDto
        {
            Id = room.Id,
            IsGameStarted = room.IsGameInProgress,
            PlayerCount = room.PlayerCount,
            RoomSetting = room.RoomSetting.ToRoomSettingDto()
        };
    }
}
