namespace Durak.Core.GameModule.ValueObjects;

public record GameResult
{
    public List<long> WinnerIds { get; set; } = [];
    public bool IsDraw { get; set; }
}