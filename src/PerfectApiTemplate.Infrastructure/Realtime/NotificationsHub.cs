using Microsoft.AspNetCore.SignalR;

namespace PerfectApiTemplate.Infrastructure.Realtime;

public sealed class NotificationsHub : Hub
{
    public Task JoinChannel(string channel)
        => Groups.AddToGroupAsync(Context.ConnectionId, channel);

    public Task LeaveChannel(string channel)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, channel);
}
