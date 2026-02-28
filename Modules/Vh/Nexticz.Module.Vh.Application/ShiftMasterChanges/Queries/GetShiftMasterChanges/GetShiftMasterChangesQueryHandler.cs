using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Common.Interfaces;


namespace Nexticz.Module.Vh.Application.ShiftMasterChanges.Queries.GetShiftMasterChanges;

public class GetShiftMasterChangesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetShiftMasterChangesQuery, ErrorOr<FilteredResult>>
{
    public async Task<ErrorOr<FilteredResult>> Handle(GetShiftMasterChangesQuery query,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.ShiftMasterChangesRepository.GetShiftMasterChangesAsync(query.FilteringParams,
            cancellationToken);
    }
}