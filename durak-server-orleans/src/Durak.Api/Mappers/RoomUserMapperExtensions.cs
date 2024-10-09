using Durak.Api.Contracts.Dtos;
using Durak.Api.Entities;

namespace Durak.Api.Mappers;

public static class RoomUserMapperExtensions
{
    public static RoomUserDto ToRoomUserDto(this RoomUser roomUser)
    {
        return new RoomUserDto
        {
            UserId = roomUser.UserId,
            IsReady = roomUser.IsReady,
            Username = roomUser.User.Username
        };
    }
}
