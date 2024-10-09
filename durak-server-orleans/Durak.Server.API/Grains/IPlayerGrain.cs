using Durak.Server.API.Entities;

namespace Durak.Server.API.Grains;

public interface IPlayerGrain : IGrainWithIntegerKey
{
    ValueTask DepositAsync(long coinCount);
    Task<string> GetCurrentRoomIdAsync();
    ValueTask<string> GetNameAsync();
    ValueTask<Player> GetPlayerAsync();
    Task SetNameAsync(string name);
    Task<bool> TryJoinRoomAsync(string roomId);
    Task<bool> TryLeaveRoomAsync();
    ValueTask<bool> TryWithdrawAsync(long coinCount);
    ValueTask UpdatePlayerStatisticAsync(bool isWin);
}
