namespace Durak.Server.API.Entities;

[GenerateSerializer]
public class Player
{
    [Id(0)]
    public long Id { get; set; }

    [Id(1)]
    public long Wins { get; set; }

    [Id(2)]
    public long Losses { get; set; }

    [Id(3)]
    public long Total { get; set; }

    [Id(4)]
    public long CoinCount { get; set; }
}
