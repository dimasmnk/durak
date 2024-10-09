namespace Durak.Server.API.Models;

[GenerateSerializer]
public class RoomPlayer(long id, string name, bool isReady, bool isConnected)
{
    [Id(0)]
    public long Id { get; set; } = id;
    [Id(1)]
    public string Name { get; set; } = name;
    [Id(2)]
    public bool IsReady { get; set; } = isReady;
    [Id(3)]
    public bool IsConnected { get; set; } = isConnected;
}
