using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.DocumentTemplateAggregate.Events;

public record DocumentTemplateCreatedEvent(Guid Id, string Code, TextOffset[] TextOffsets, TextBackground[] TextBackgrounds, DateTimeOffset CreatedAt) : IMartenEvent;