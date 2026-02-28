using ErrorOr;
using MediatR;

namespace Nexticz.Module.Portal.Application.Modules.Queries.GetModuleById;

internal record GetModuleByIdQuery(Guid Id) : IRequest<ErrorOr<Domain.ModuleAggregate.Module>>;