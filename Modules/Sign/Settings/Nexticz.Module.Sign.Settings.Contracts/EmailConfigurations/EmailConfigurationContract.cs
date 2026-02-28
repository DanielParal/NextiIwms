namespace Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;

public record EmailConfigurationContract(
    Guid Id,
    string[] DepositorCodes, 
    string[] PartnerCodes, 
    string[] ReceiverAndPartnerCombinationCodes, 
    bool ShouldSendDeliveryDocument, 
    bool ShouldSendLoadingDocument, 
    string[] RecipientEmailAddresses, 
    bool ShouldSendImmediately,
    EmailConfigurationTypeContract TypeContract);