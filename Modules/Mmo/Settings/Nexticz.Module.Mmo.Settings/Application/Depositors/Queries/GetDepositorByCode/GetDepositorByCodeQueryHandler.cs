using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.Interfaces;
using Nexticz.Module.Mmo.Settings.Domain.DepositorEntity;

namespace Nexticz.Module.Mmo.Settings.Application.Depositors.Queries.GetDepositorByCode;

internal class GetDepositorByCodeQueryHandler (IDepositorReadOnlyRepository depositorReadOnlyRepository)
    : IRequestHandler<GetDepositorByCodeQuery, ErrorOr<Depositor>>
{
    public async Task<ErrorOr<Depositor>> Handle(GetDepositorByCodeQuery request, CancellationToken cancellationToken)
    {
        var depositor = await depositorReadOnlyRepository
            .GetByCodeAsync(request.Code, cancellationToken);
        
        if (depositor is null)
            return DepositorErrors.DepositorWithCodeDoesNotExist;

        return depositor;
    }
}