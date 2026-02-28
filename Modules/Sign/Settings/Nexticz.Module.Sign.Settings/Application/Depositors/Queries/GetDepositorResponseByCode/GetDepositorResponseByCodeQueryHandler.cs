using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Contracts.Depositors;
using Nexticz.Module.Sign.Settings.Contracts.Depositors.Queries;
using Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorByCode;

namespace Nexticz.Module.Sign.Settings.Application.Depositors.Queries.GetDepositorResponseByCode;

internal class GetDepositorResponseByCodeQueryHandler(ISender sender) : IRequestHandler<GetDepositorResponseByCodeQuery, ErrorOr<DepositorResponse>>
{
    public async Task<ErrorOr<DepositorResponse>> Handle(GetDepositorResponseByCodeQuery request, CancellationToken cancellationToken)
    {
        var depositor = await sender.Send(new GetDepositorByCodeQuery(request.Code), cancellationToken);
        
        if (depositor.IsError)
            return depositor.Errors;
        
        return DepositorResponseFactory.Create(depositor.Value);
    }
}