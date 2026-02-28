using Microsoft.AspNetCore.SignalR; 
using Nexticz.Lib.Shared.UserProviders;

namespace Nexticz.Module.Notifications.Application.SignalRNotifications;

public class SignalRHub(
    ICurrentUserProvider currentUserProvider) : Hub
{
    private string UserName { get; } = currentUserProvider.GetCurrentUser().UserName;

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var groupId = httpContext?.Request.Query["groupId"];

        if (!string.IsNullOrWhiteSpace(groupId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId!);
            await base.OnConnectedAsync();
            return;       
        }

        if (!string.IsNullOrWhiteSpace(UserName))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserName);
            await base.OnConnectedAsync();
            return;  
        }
        
        await base.OnConnectedAsync();
    }
}