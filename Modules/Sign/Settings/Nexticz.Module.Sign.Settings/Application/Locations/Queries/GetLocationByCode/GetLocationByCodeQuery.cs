using ErrorOr;
using MediatR;
using Nexticz.Module.Sign.Settings.Domain.LocationAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Locations.Queries.GetLocationByCode;

internal record GetLocationByCodeQuery(string Code) : IRequest<ErrorOr<Location>>; 