using ErrorOr;

namespace Nexticz.Module.Portal.Application.Modules.Commands.DeleteModule;

internal record DeleteModuleCommand(Guid Id) : IPortalCommand<ErrorOr<Success>>;