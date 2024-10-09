using System.Collections.Concurrent;

namespace Durak.Server.API.Services.Interfaces;
public interface ISignalrConnectionService
{
    ConcurrentDictionary<long, string> PlayerConnections { get; }

    void AddConnection(long playerId, string connectionId);
    string GetConnection(long playerId);
    void RemoveConnection(long playerId);
}