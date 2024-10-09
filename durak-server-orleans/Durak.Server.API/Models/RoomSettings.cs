using Durak.Server.API.Enums;

namespace Durak.Server.API.Models;

[GenerateSerializer]
public class RoomSettings(Guid id, Bet bet, bool isPrivate)
{
    [Id(0)]
    public Guid Id { get; set; } = id;
    [Id(1)]
    public Bet Bet { get; set; } = bet;
    [Id(2)]
    public bool IsPrivate { get; set; } = isPrivate;
}
