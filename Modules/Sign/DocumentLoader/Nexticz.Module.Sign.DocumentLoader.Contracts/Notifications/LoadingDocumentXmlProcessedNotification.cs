using MediatR;

namespace Nexticz.Module.Sign.DocumentLoader.Contracts.Notifications;

public record LoadingDocumentXmlProcessedNotification(LoadingDocumentContract LoadingDocument) : INotification;