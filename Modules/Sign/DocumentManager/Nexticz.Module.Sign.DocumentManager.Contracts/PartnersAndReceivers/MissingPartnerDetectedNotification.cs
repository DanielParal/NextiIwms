using MediatR;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.PartnersAndReceivers;

public record MissingPartnerDetectedNotification(string Code, string Name) : INotification;