using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Lib.Shared.UserProviders;
using Interfaces_IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;
using IUnitOfWork = Nexticz.Module.Vh.Application.Common.Interfaces.IUnitOfWork;

namespace Nexticz.Module.Vh.Application.Centers.Queries.ListCenters;

public class ListCentersQueryHandler(Interfaces_IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    : IRequestHandler<ListCentersQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(ListCentersQuery query, CancellationToken cancellationToken)
    {
        if (query.FilteringParams.Skip is not null)
            return await unitOfWork.CentersRepository.GetCentersResponseAsync(query.FilteringParams, cancellationToken);

        var currentUser = currentUserProvider.GetCurrentUser();
        var vhUser = (await unitOfWork.VhUsersRepository.GetVhUserByIdAsync(currentUser.Id, cancellationToken))
            ?.Centers
            ?.Select(x => x.Code)
            .ToList() ?? [];

        var filter = "[";
        var index = 0;

        foreach (var code in vhUser)
        {
            filter += $"[\"code\",\"=\",\"{code}\"]";
            index++;

            filter += index == vhUser.Count ? "" : ",\"or\",";
        }

        filter += "]";

        query.FilteringParams.Filter = filter;

        return await unitOfWork.CentersRepository.GetCentersResponseAsync(query.FilteringParams, cancellationToken);
    }
}