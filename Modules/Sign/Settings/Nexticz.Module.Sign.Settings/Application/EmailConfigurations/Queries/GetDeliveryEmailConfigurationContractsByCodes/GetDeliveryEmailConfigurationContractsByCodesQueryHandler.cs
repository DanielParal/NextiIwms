using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations.Queries;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetDeliveryEmailConfigurationContractsByCodes;

internal class GetDeliveryEmailConfigurationContractsByCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository)  : IRequestHandler<GetDeliveryEmailConfigurationContractsByCodesQuery, EmailConfigurationContract[]>
{
    public async Task<EmailConfigurationContract[]> Handle(GetDeliveryEmailConfigurationContractsByCodesQuery request, CancellationToken cancellationToken)
    {
        var upperDepositorCode = request.DepositorCode.ToUpperInvariant();
        var upperPartnerCode = request.PartnerCode.ToUpperInvariant();
        var receiverAndPartnerCombination = EmailConfiguration.ComposeReceiverAndPartnerCombinationCodes(request.ReceiverCode, request.PartnerCode);

        var emailConfigurations = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<EmailConfiguration>(
                x => 
                    x.Type == EmailConfigurationType.DeliveryConfiguration &&
                    (x.DepositorCodes.Length == 0 || x.DepositorCodes.Contains(upperDepositorCode)) &&
                    (x.PartnerCodes.Length == 0 || x.PartnerCodes.Contains(upperPartnerCode)) &&
                    (x.ReceiverAndPartnerCombinationCodes.Length == 0 || x.ReceiverAndPartnerCombinationCodes.Contains(receiverAndPartnerCombination)), 
                cancellationToken);
        
        return emailConfigurations.Select(EmailConfigurationContractFactory.Create).ToArray();
    }
}