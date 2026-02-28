using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Vh.Application.Partners.Common.Models;


namespace Nexticz.Module.Vh.Application.Partners.Queries.GetPartners;

public class GetPartnersQuery : IRequest<ErrorOr<FilteredResult>>
{
    public required PartnersFilteringParams FilteringParams { get; set; }
}