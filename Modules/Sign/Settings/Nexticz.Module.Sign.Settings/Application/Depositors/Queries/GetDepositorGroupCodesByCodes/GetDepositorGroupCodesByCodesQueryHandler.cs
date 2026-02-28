using Marten;
using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorGroupCodesByCodes;

internal class GetDepositorGroupCodesByCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetDepositorGroupCodesByCodesQuery, string[]>
{
    public async Task<string[]> Handle(GetDepositorGroupCodesByCodesQuery request, CancellationToken cancellationToken)
    {
        var upperCodes = request.Codes.Select(x => x.ToUpperInvariant()).ToArray();
        var depositors = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Depositor>(
                x => x.Code.IsOneOf(upperCodes), 
                cancellationToken);

        return depositors.Select(x => x.DepositorGroupCode).Distinct().ToArray();
    }
}