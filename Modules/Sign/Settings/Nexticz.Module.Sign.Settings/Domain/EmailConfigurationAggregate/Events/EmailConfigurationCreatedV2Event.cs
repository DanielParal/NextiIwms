using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate.Events;

public record EmailConfigurationCreatedV2Event(
    Guid Id, string[] DepositorCodes, string[] PartnerCodes, string[] ReceiverAndPartnerCombinationCodes, bool ShouldSendDeliveryDocument, bool ShouldSendLoadingDocument,
    string[] RecipientEmailAddresses, bool ShouldSendImmediately, EmailConfigurationType Type) : IMartenEvent;