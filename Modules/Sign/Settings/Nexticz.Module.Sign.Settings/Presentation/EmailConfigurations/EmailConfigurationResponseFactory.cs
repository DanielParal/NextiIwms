using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Lib.Shared.Emails;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Presentation.EmailConfigurations;

internal static class EmailConfigurationResponseFactory
{
    public static EmailConfigurationResponse Create(EmailConfiguration emailConfiguration)
    {
        return new EmailConfigurationResponse(
            emailConfiguration.Id,
            emailConfiguration.DepositorCodes,
            emailConfiguration.DepositorCodesInString,
            emailConfiguration.PartnerCodes,
            emailConfiguration.PartnerCodesInString,
            emailConfiguration.ReceiverAndPartnerCombinationCodes,
            emailConfiguration.ReceiverAndPartnerCombinationCodesInString,
            emailConfiguration.ShouldSendDeliveryDocument,
            emailConfiguration.ShouldSendLoadingDocument,
            emailConfiguration.RecipientEmailAddresses,
            emailConfiguration.ShouldSendImmediately,
            (EmailConfigurationTypeContract)emailConfiguration.Type);
    }
}