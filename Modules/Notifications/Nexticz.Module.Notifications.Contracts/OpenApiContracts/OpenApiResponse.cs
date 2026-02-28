using System.ComponentModel.DataAnnotations;
using Nexticz.Module.Notifications.Contracts.SignalRNotifications;

namespace Nexticz.Module.Notifications.Contracts.OpenApiContracts;

public record OpenApiResponse(
    [property: Required] SignalRReceiveNotificationNameContract SignalRReceiveNotificationName,
    [property: Required] SignalRNotificationResponse SignalRNotificationResponse);