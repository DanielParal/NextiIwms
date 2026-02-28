using ErrorOr;

namespace Nexticz.Module.Sign.Settings.Application.Users.Commands.DeleteUserSignature;

internal record DeleteUserSignatureCommand(string UserName) : ISettingsCommand<ErrorOr<Success>>;