using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorsByDepositorGroupCode;

internal class GetDepositorsByDepositorGroupCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDepositorsByDepositorGroupCodeQuery, Depositor[]>
{
    public async Task<Depositor[]> Handle(GetDepositorsByDepositorGroupCodeQuery request, CancellationToken cancellationToken)
    {
        var upperCode = request.DepositorGroupCode.ToUpperInvariant();;
        var depositors = await readOnlyEventStoreRepository
            .GetAllByConditionAsync<Depositor>(
                x => x.DepositorGroupCode.Equals(upperCode), 
                cancellationToken);

        return depositors.ToArray();
    }
}