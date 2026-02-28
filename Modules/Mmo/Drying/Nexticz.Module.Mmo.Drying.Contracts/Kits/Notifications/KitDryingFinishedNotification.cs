using MediatR;

namespace Nexticz.Module.Mmo.Drying.Contracts.Kits.Notifications;

public record KitDryingFinishedNotification(KitContract Kit) : INotification;