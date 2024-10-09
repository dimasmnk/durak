using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Durak.Api.Hubs;

[Authorize]
public class RoomListHub : Hub
{
}
