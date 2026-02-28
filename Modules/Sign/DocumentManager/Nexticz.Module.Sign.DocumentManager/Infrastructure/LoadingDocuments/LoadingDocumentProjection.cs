using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate.Events;

namespace Nexticz.Module.Sign.DocumentManager.Infrastructure.LoadingDocuments;

public class LoadingDocumentProjection : SingleStreamProjection<LoadingDocument, Guid>
{
    public void Apply(IEvent<LoadingDocumentCreatedEvent> @event, LoadingDocument loadingDocument)
    {
        loadingDocument.Apply(@event.Data);
    }
    
    public void Apply(IEvent<LoadingDocumentToSigningDeviceSentEvent> @event, LoadingDocument loadingDocument)
    {
        loadingDocument.Apply(@event.Data);
    }
    
    public void Apply(IEvent<LoadingDocumentFromSigningDeviceReturnedEvent> @event, LoadingDocument loadingDocument)
    {
        loadingDocument.Apply(@event.Data);
    }
    
    public void Apply(IEvent<SignedDocumentManuallyUploadedEvent> @event, LoadingDocument loadingDocument)
    {
        loadingDocument.Apply(@event.Data);
    }
    
    public void Apply(IEvent<LoadingDocumentSignedEvent> @event, LoadingDocument loadingDocument)
    {
        loadingDocument.Apply(@event.Data);
    }
    
    
    public void Apply(IEvent<LoadingDocumentEmailSentEvent> @event, LoadingDocument loadingDocument)
    {
        loadingDocument.Apply(@event.Data);
    }
    
    public void Apply(IEvent<LoadingDocumentPrintCopiesCountChangedEvent> @event, LoadingDocument loadingDocument)
    {
        loadingDocument.Apply(@event.Data);
    }
}