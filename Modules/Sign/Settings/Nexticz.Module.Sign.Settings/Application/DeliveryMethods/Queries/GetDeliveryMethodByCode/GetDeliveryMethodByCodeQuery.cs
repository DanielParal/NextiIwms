using MediatR;
using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.DeliveryMethodAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.Queries.GetDeliveryMethodByCode;

internal record GetDeliveryMethodByCodeQuery(string Code) : IRequest<ErrorOr<DeliveryMethod>>; 