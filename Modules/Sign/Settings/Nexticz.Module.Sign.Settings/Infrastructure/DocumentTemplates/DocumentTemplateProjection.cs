using JasperFx.Events;
using Marten.Events.Aggregation;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate;
using Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate.Events;

namespace Nexticz.Module.Sign.Settings.Infrastructure.DocumentTemplates;

public class DocumentTemplateProjection : SingleStreamProjection<DocumentTemplate, Guid>
{
    public DocumentTemplateProjection()
    {
        DeleteEvent<DocumentTemplateDeletedEvent>();
    }
    
    public void Apply(IEvent<DocumentTemplateCreatedEvent> @event, DocumentTemplate documentTemplate)
    {
        documentTemplate.Apply(@event.Data);
    }
    
    public void Apply(IEvent<DocumentTemplateUpdatedEvent> @event, DocumentTemplate documentTemplate)
    {
        documentTemplate.Apply(@event.Data);
    }
}