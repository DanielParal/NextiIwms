using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations;
using Nexticz.Module.Sign.Settings.Contracts.EmailConfigurations.Queries;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetLoadingEmailConfigurationContractByDepositorCode;

internal class GetLoadingEmailConfigurationContractByDepositorCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetLoadingEmailConfigurationContractByDepositorCodeQuery, ErrorOr<EmailConfigurationContract>>
{
    public async Task<ErrorOr<EmailConfigurationContract>> Handle(GetLoadingEmailConfigurationContractByDepositorCodeQuery request, CancellationToken cancellationToken)
    {
        var upperDepositorCode = request.DepositorCode.ToUpperInvariant();
        var emailConfiguration = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<EmailConfiguration>(
                x => 
                    x.DepositorCodes.Contains(upperDepositorCode) && 
                    x.Type == EmailConfigurationType.LoadingConfiguration, 
                cancellationToken);

        if (emailConfiguration is null)
            return EmailConfigurationErrors.LoadingConfigurationNotFound;
        
        return EmailConfigurationContractFactory.Create(emailConfiguration);
    }
}