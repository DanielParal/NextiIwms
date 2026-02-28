using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.UserAggregate.Events;

public record UserSignatureUploadedEvent(Guid Id, string UserName) : IMartenEvent;