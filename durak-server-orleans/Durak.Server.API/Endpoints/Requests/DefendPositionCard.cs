using Durak.Server.API.Game.Entities;

namespace Durak.Server.API.Endpoints.Requests;

public class DefendPositionCard(int attackCardId, Card defendCard)
{
    public int AttackCardId { get; set; } = attackCardId;
    public Card DefendCard { get; set; } = defendCard;
}
