using Marten;
using MediatR;
using Nexticz.Module.Sign.Settings.Contracts.Depositors;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorResponsesByCodesAndGroupCodes;

internal class GetDepositorResponsesByCodesAndGroupCodesQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) : IRequestHandler<GetDepositorResponsesByCodesAndGroupCodesQuery, DepositorResponse[]>
{
    public async Task<DepositorResponse[]> Handle(GetDepositorResponsesByCodesAndGroupCodesQuery request, CancellationToken cancellationToken)
    {
        var upperCodes = request.Codes.Select(x => x.ToUpperInvariant()).ToArray();
        var upperGroupCodes = request.GroupCodes.Select(x => x.ToUpperInvariant()).ToArray();
        var depositors = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Depositor>(
                x => x.DepositorGroupCode.IsOneOf(upperGroupCodes) || x.Code.IsOneOf(upperCodes), 
                cancellationToken);
        
        return depositors.Select(DepositorResponseFactory.Create).ToArray();
    }
}