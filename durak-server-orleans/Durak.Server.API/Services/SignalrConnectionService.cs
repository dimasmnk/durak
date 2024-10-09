using Durak.Server.API.Services.Interfaces;
using System.Collections.Concurrent;

namespace Durak.Server.API.Services;

public class SignalrConnectionService : ISignalrConnectionService
{
    public ConcurrentDictionary<long, string> PlayerConnections { get; } = new();

    public void AddConnection(long playerId, string connectionId)
    {
        PlayerConnections.AddOrUpdate(playerId, connectionId, (_, _) => connectionId);
    }

    public void RemoveConnection(long playerId)
    {
        PlayerConnections.TryRemove(playerId, out _);
    }

    public string GetConnection(long playerId)
    {
        return PlayerConnections.TryGetValue(playerId, out var connectionId) ? connectionId : string.Empty;
    }
}
