using Durak.Core.GameModule.Enums;
using Durak.Core.GameModule.Exceptions;
using Durak.Core.GameModule.ValueObjects;

namespace Durak.Core.GameModule.Entities;

public class Player
{
    public long Id { get; set; }
    public PlayerStatus Status { get; private set; }
    public bool IsPass => Status == PlayerStatus.Pass;
    public bool IsTake => Status == PlayerStatus.Take;

    public List<Card> Hand { get; set; } = [];

    public void SetPassStatus()
    {
        Status = PlayerStatus.Pass;
    }

    public void SetTakeStatus()
    {
        Status = PlayerStatus.Take;
    }

    public void ResetStatus()
    {
        Status = PlayerStatus.None;
    }

    public Card DrawCard(Card card)
    {
        ValidatePlayerHasCard(card);
        Hand.Remove(card);
        return card;
    }

    private void ValidatePlayerHasCard(Card card)
    {
        var isCardExist = Hand.Contains(card);

        if (!isCardExist) throw new CardNotFoundForPlayerException(Id, card);
    }
}