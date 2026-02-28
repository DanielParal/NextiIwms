using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.EmailConfigurationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.EmailConfigurations.Queries.GetEmailConfigurationByExactCodes;

internal class GetEmailConfigurationByExactCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetEmailConfigurationByExactCodesQuery, ErrorOr<EmailConfiguration>>
{
    public async Task<ErrorOr<EmailConfiguration>> Handle(GetEmailConfigurationByExactCodesQuery request, CancellationToken cancellationToken)
    {
        var upperDepositorCodes = request.DepositorCodes.Select(x => x.ToUpperInvariant()).ToArray();
        var upperPartnerCodes = request.PartnerCodes.Select(x => x.ToUpperInvariant()).ToArray();
        var upperReceiverCodes = request.ReceiverAndPartnerCombinationCodes.Select(x => x.ToUpperInvariant()).ToArray();
        
        var candidates = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<EmailConfiguration>(
                x => 
                    x.DepositorCodes.Length == upperDepositorCodes.Length &&
                    x.PartnerCodes.Length == upperPartnerCodes.Length &&
                    x.ReceiverAndPartnerCombinationCodes.Length == upperReceiverCodes.Length,
                cancellationToken);
        
        var emailConfiguration = candidates.FirstOrDefault(x =>
            x.DepositorCodes.OrderBy(c => c).SequenceEqual(upperDepositorCodes.OrderBy(c => c)) &&
            x.PartnerCodes.OrderBy(c => c).SequenceEqual(upperPartnerCodes.OrderBy(c => c)) &&
            x.ReceiverAndPartnerCombinationCodes.OrderBy(c => c).SequenceEqual(upperReceiverCodes.OrderBy(c => c)));

        if (emailConfiguration is null)
            return EmailConfigurationErrors.CodesCombinationNotFound;
        
        return emailConfiguration;
    }
}