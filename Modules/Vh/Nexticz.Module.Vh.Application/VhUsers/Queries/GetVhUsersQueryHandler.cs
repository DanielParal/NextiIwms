using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.VhUsers.Queries;

public class GetVhUsersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetVhUsersQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetVhUsersQuery query, CancellationToken cancellationToken)
    {
        return await unitOfWork.VhUsersRepository.GetVhUsersAsync(query.FilteringParams,
            cancellationToken);
    }
}