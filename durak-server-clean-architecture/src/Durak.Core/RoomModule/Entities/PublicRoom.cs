using Durak.Core.RoomModule.Enums;
using Durak.Core.RoomModule.ValueObjects;

namespace Durak.Core.RoomModule.Entities;

public class PublicRoom : BetRoom<PublicRoomSettings>
{
    private PublicRoom(PublicRoomSettings settings) : base(settings)
    {
    }

    public override RoomType Type => RoomType.Public;

    public static PublicRoom CreateRoom(PublicRoomSettings publicRoomSettings)
    {
        return new PublicRoom(publicRoomSettings);
    }
}