using Durak.Api.Contracts.Dtos;
using Durak.Api.Entities;

namespace Durak.Api.Mappers;

public static class RoomSettingMapperExtensions
{
    public static RoomSettingDto ToRoomSettingDto(this RoomSetting roomSetting)
    {
        return new RoomSettingDto
        {
            Bet = roomSetting.Bet,
            IsPublic = roomSetting.IsPublic,
            MaxPlayerCount = roomSetting.MaxPlayerCount
        };
    }
}
