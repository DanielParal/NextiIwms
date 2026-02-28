using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.ShiftMasterChanges.Common.Models;


namespace Nexticz.Module.Vh.Application.ShiftMasterChanges.Queries.GetShiftMasterChanges;

public class GetShiftMasterChangesQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required ShiftMasterChangesFilteringParams FilteringParams { get; set; }
}