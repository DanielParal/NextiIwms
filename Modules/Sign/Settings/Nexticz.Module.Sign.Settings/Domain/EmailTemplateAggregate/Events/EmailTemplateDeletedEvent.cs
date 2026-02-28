using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.EmailTemplateAggregate.Events;

public record EmailTemplateDeletedEvent(Guid Id, string Code) : IMartenEvent;