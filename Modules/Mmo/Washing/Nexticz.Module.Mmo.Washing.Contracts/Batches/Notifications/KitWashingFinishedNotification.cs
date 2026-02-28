using MediatR;

namespace Nexticz.Module.Mmo.Washing.Contracts.Batches.Notifications;

public record KitWashingFinishedNotification(
    KitWashingFinishedContract FinishedKit, 
    KitWashingFinishedContract? FinishedSisterKit) : INotification;