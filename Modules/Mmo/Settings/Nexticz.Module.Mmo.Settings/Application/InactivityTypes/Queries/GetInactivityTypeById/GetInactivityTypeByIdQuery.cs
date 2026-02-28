using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeById;

internal record GetInactivityTypeByIdQuery(Guid Id) : IRequest<ErrorOr<InactivityType>>;