using Nexticz.Lib.Shared.DataAccess.Marten.Configuration;

namespace Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate.Events;

public record EmailConfigurationUpdatedEvent(
    Guid Id, string? DepositorCode, string? PartnerCode, string? ReceiverCode, bool ShouldSendDeliveryDocument, bool ShouldSendLoadingDocument,
    string[] RecipientEmailAddresses, bool ShouldSendImmediately, EmailConfigurationType Type) : IMartenEvent;