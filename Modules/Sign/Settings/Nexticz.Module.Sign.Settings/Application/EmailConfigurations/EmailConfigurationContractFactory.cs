using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations;

internal static class EmailConfigurationContractFactory
{
    public static EmailConfigurationContract Create(EmailConfiguration emailConfiguration)
    {
        return new EmailConfigurationContract(
            emailConfiguration.Id,
            emailConfiguration.DepositorCodes,
            emailConfiguration.PartnerCodes,
            emailConfiguration.ReceiverAndPartnerCombinationCodes,
            emailConfiguration.ShouldSendDeliveryDocument,
            emailConfiguration.ShouldSendLoadingDocument,
            emailConfiguration.RecipientEmailAddresses,
            emailConfiguration.ShouldSendImmediately,
            (EmailConfigurationTypeContract) emailConfiguration.Type);
    }
}