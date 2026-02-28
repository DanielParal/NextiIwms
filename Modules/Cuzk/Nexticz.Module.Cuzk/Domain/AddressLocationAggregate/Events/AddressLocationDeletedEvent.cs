using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Cuzk.Domain.AddressLocationAggregate.Events;

public record AddressLocationDeletedEvent(Guid Id, string AdmCode, string MunicipalityCode, DateTimeOffset DeletedAt) : IMartenEvent;