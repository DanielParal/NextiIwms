using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Cuzk.Domain.MunicipalityAggregate.Events;

public record MunicipalityDeletedEvent(Guid Id, string Code, DateTimeOffset DeletedAt) : IMartenEvent;