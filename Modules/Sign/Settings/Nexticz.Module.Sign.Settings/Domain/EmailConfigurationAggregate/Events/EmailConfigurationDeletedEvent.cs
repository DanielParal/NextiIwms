using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate.Events;

public record EmailConfigurationDeletedEvent(
    Guid Id, string? DepositorCode, string? PartnerCode, string? ReceiverCode) : IMartenEvent;