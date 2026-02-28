using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Nexticz.Module.Mmo.Settings.Application.Kits.Commands.UploadKitInstruction;

internal record UploadKitInstructionCommand(string KitCode, IFormFile FormFile) : ISettingsCommand<ErrorOr<Success>>;