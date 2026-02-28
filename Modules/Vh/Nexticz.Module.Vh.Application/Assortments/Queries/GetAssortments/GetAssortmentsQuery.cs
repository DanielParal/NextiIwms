using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Assortments.Common.Models;


namespace Nexticz.Module.Vh.Application.Assortments.Queries.GetAssortments;

public class GetAssortmentsQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required AssortmentsFilteringParams FilteringParams { get; set; }
}