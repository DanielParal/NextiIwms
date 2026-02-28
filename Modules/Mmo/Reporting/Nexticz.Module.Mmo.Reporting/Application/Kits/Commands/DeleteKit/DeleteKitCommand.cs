using ErrorOr;

namespace Nexticz.Module.Mmo.Reporting.Application.Kits.Commands.DeleteKit;

internal record DeleteKitCommand(Guid Id) : IReportingCommand<ErrorOr<Success>>;