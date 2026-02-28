using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Reporting.Domain.DriedKitAggregate;

namespace Nexticz.Module.Mmo.Reporting.Application.DriedKits.Queries.GetDriedKitByIdFromDrying;

internal record GetDriedKitByIdFromDryingQuery(Guid IdFromDying) : IRequest<ErrorOr<DriedKit>>;