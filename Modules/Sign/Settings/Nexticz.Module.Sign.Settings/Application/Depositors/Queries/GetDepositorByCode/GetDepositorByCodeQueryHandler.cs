using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Application.Interfaces;
using Nexticz.Module.Sign.Settings.Domain.DepositorAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorByCode;

internal class GetDepositorByCodeQueryHandler(
    ISettingsReadOnlyEventStoreRepository readOnlyEventStoreRepository) 
    : IRequestHandler<GetDepositorByCodeQuery, ErrorOr<Depositor>>
{
    public async Task<ErrorOr<Depositor>> Handle(GetDepositorByCodeQuery request, CancellationToken cancellationToken)
    {
        var partner = await readOnlyEventStoreRepository
            .GetFirstByConditionAsync<Depositor>(
                x => x.Code.Equals(request.Code, StringComparison.InvariantCultureIgnoreCase), 
                cancellationToken);

        if (partner is null)
            return DepositorErrors.CodeDoesNotExist;
        
        return partner;
    }
}