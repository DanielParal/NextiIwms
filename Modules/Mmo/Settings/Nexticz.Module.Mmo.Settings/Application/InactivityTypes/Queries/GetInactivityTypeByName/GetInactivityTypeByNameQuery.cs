using MediatR;
using ErrorOr;
using Nexticz.Module.Mmo.Settings.Domain.InactivityTypeEntity;

namespace Nexticz.Module.Mmo.Settings.Application.InactivityTypes.Queries.GetInactivityTypeByName;

internal record GetInactivityTypeByNameQuery(string Name) : IRequest<ErrorOr<InactivityType>>;