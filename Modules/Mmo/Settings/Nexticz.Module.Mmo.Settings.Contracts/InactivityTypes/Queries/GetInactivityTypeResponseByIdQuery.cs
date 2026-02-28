using MediatR;
using ErrorOr;

namespace Nexticz.Module.Mmo.Settings.Contracts.InactivityTypes.Queries;

public record GetInactivityTypeResponseByIdQuery(Guid Id) : IRequest<ErrorOr<InactivityTypeResponse>>;