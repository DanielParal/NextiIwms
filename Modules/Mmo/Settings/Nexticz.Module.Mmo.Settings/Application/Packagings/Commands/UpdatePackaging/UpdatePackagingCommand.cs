using ErrorOr;
using MediatR;
using Nexticz.Module.Mmo.Settings.Contracts.Packagings;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.UpdatePackaging;

internal record UpdatePackagingCommand(
    string Code,
    UpdatePackagingRequest UpdatePackagingRequest) : ISettingsCommand<ErrorOr<Updated>>;