using ErrorOr;
using Nexticz.Module.Portal.Contracts.Modules;
using Nexticz.Module.Portal.Domain.ModuleAggregate;

namespace Nexticz.Module.Portal.Application.Modules.Commands.CreateModule;

internal record CreateModuleCommand(CreateModuleRequest Request) : IPortalCommand<ErrorOr<Domain.ModuleAggregate.Module>>;