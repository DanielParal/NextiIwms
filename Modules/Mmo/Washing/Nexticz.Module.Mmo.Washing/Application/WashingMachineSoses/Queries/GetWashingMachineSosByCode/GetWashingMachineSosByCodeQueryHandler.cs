using MediatR;
using Nexticz.Module.Mmo.Washing.Application.Interfaces;
using Nexticz.Module.Mmo.Washing.Domain.WashingMachineSosAggregate;

namespace Nexticz.Module.Mmo.Washing.Application.WashingMachineSoses.Queries.GetWashingMachineSosByCode;

internal class GetWashingMachineSosByCodeQueryHandler(
    IWashingMachineSosReadOnlyRepository washingMachineSosReadOnlyRepository) : IRequestHandler<GetWashingMachineSosByCodeQuery, WashingMachineSos?>
{
    public async Task<WashingMachineSos?> Handle(GetWashingMachineSosByCodeQuery request, CancellationToken cancellationToken)
    {
        return await washingMachineSosReadOnlyRepository.GetWashingMachineSosByCodeAsync(request.Code, cancellationToken);
    }
}