using Durak.Core.RoomModule.Enums;
using Durak.Core.RoomModule.ValueObjects;

namespace Durak.Core.RoomModule.Entities;

public class PrivateRoom : BetRoom<PrivateRoomSettings>
{
    private PrivateRoom(PrivateRoomSettings privateRoomSettings) : base(privateRoomSettings)
    {
    }

    public override RoomType Type => RoomType.Private;

    public static PrivateRoom CreateRoom(PrivateRoomSettings privateRoomSettings)
    {
        return new PrivateRoom(privateRoomSettings);
    }
}