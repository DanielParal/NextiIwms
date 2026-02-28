using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.PartnerAggregate;


namespace Nexticz.Module.Sign.Settings.Application.Partners.Queries.GetPartners;

internal record GetPartnersQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<Partner>>;