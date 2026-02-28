using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Centers.Common.Models;


namespace Nexticz.Module.Vh.Application.Centers.Queries.ListCenters;

public class ListCentersQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required CentersFilteringParams FilteringParams { get; set; }
}