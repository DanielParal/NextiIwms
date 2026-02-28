using ErrorOr;
using MediatR;
using Nexticz.Module.Portal.Domain.ModuleAggregate;

namespace Nexticz.Module.Portal.Application.Modules.Queries.GetModuleByName;

internal record GetModuleByNameQuery(string Name) : IRequest<ErrorOr<Domain.ModuleAggregate.Module>>;