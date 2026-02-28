using ErrorOr;

namespace Nexticz.Module.Portal.Application.Modules.Commands.ChangeModuleOrder;

internal record ChangeModuleOrderCommand(Guid Id, int ToOrder) : IPortalCommand<ErrorOr<Success>>;