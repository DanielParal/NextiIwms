using ErrorOr;
using Nexticz.Module.Sign.Settings.Domain.DepositorGroupAggregate;

namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.Commands.CreateDepositorGroup;

internal record CreateDepositorGroupCommand(string Code, string Name) : ISettingsCommand<ErrorOr<DepositorGroup>>;