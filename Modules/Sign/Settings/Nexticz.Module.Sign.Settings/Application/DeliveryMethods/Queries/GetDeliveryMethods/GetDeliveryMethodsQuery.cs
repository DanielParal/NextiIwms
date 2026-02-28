using MediatR;
using Nexticz.Lib.Shared.DevExtreme;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;


namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethods;

internal record GetDeliveryMethodsQuery(BaseFilteringParams FilteringParams) : IRequest<FilteredResult<DeliveryMethod>>;