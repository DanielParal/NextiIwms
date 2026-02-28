using System.ComponentModel.DataAnnotations;

namespace Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;

public record UpdateEmailConfigurationRequest(
    [property: Required] string[] DepositorCodes, 
    [property: Required] string[] PartnerCodes, 
    [property: Required] string[] ReceiverAndPartnerCombinationCodes, 
    [property: Required] bool ShouldSendDeliveryDocument, 
    [property: Required] bool ShouldSendLoadingDocument, 
    [property: Required] string[] RecipientEmailAddresses, 
    [property: Required] bool ShouldSendImmediately);