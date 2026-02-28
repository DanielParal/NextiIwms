using ErrorOr;
using Nexticz.Module.Portal.Contracts.Modules;

namespace Nexticz.Module.Portal.Application.Modules.Commands.UpdateModule;

internal record UpdateModuleCommand(Guid Id, UpdateModuleRequest Request) : IPortalCommand<ErrorOr<Success>>;