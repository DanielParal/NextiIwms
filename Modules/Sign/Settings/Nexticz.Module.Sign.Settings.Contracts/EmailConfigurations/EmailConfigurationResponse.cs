using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;

public record EmailConfigurationResponse(
    [property: Required] Guid Id,
    [property: Required] string[] DepositorCodes, 
    [property: Required] string DepositorCodesInString, 
    [property: Required] string[] PartnerCodes, 
    [property: Required] string PartnerCodesInString, 
    [property: Required] string[] ReceiverAndPartnerCombinationCodes, 
    [property: Required] string ReceiverAndPartnerCombinationCodesInString, 
    [property: Required] bool ShouldSendDeliveryDocument, 
    [property: Required] bool ShouldSendLoadingDocument, 
    [property: Required] string[] RecipientEmailAddresses,
    [property: Required] bool ShouldSendImmediately,
    [property: Required] EmailConfigurationTypeContract TypeContract);