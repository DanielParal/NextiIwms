using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate.Events;

public record EmailConfigurationDeletedV2Event(
    Guid Id, string[] DepositorCodes, string[] PartnerCodes, string[] ReceiverAndPartnerCombinationCodes) : IMartenEvent;