using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Mmo.Settings.Contracts.Users;

public record UpdateUserRequest(
    [property: Required] ReceivableNotificationContract[] ReceivableNotifications);