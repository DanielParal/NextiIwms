using ErrorOr;
using MediatR;

namespace Nexticz.Module.Mmo.Settings.Application.Packagings.Commands.DeletePackaging;

internal record DeletePackagingCommand(string Code) : ISettingsCommand<ErrorOr<Deleted>>;