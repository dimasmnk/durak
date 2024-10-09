using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Durak.Server.API.Hubs;

[Authorize]
public class RoomListHub : Hub
{
}
