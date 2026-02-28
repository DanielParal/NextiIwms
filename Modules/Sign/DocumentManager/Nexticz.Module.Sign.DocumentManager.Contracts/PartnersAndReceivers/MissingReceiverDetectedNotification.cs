using MediatR;

namespace Nexticz.Module.Sign.DocumentManager.Contracts.PartnersAndReceivers;

public record MissingReceiverDetectedNotification(string ReceiverCode, string PartnerCode, string Name) : INotification;