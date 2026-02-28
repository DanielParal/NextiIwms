using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes;
using Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes.Queries;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeById;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeResponseById;

internal class GetInactivityTypeResponseByIdQueryHandler(
    ISender sender) : IRequestHandler<GetInactivityTypeResponseByIdQuery, ErrorOr<InactivityTypeResponse>>
{
    public async Task<ErrorOr<InactivityTypeResponse>> Handle(GetInactivityTypeResponseByIdQuery request, CancellationToken cancellationToken)
    {
        var inactivityType = await sender.Send(new GetInactivityTypeByIdQuery(request.Id), cancellationToken);
        
        if (inactivityType.IsError)
            return inactivityType.Errors;
        
        return InactivityTypeResponseFactory.Create(inactivityType.Value);
    }
}