using Durak.Server.API.Game.Enums;

namespace Durak.Server.API.Game.Entities;

[GenerateSerializer]
public record Card(Rank Rank, Suit Suit)
{
    public Card() : this(default, default)
    {

    }
}
