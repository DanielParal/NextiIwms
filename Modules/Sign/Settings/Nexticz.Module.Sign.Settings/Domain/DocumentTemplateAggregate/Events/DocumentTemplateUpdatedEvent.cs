using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate.Events;

public record DocumentTemplateUpdatedEvent(Guid Id, TextOffset[] TextOffsets, TextBackground[] TextBackgrounds, DateTimeOffset UpdatedAt) : IMartenEvent;