using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

public record LoadingDocumentWithAllDeliveryDocumentsFinishedEvent(Guid Id, string Code) : IMartenEvent;