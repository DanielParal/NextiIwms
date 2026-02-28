using Marten;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorsByCodes;

internal class GetDepositorsByCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDepositorsByCodesQuery, Depositor[]>
{
    public async Task<Depositor[]> Handle(GetDepositorsByCodesQuery request, CancellationToken cancellationToken)
    {
        var upperCodes = request.Codes.Select(x => x.ToUpperInvariant()).ToArray();
        var depositors = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Depositor>(
                x => x.Code.IsOneOf(upperCodes), 
                cancellationToken);

        return depositors.ToArray();
    }
}