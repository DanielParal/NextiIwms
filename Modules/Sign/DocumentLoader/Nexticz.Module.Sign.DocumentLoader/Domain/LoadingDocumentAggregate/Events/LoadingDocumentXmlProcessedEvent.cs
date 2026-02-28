using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentLoader.Domain.LoadingDocumentAggregate.Events;

public record LoadingDocumentXmlProcessedEvent(Guid Id, string Code, string[] DeliveryDocumentCodes) : IMartenEvent;