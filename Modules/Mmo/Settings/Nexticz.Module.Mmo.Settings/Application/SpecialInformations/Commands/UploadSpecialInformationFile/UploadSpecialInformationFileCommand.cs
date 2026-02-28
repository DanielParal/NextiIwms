using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Nexticz.Module.Mmo.Settings.Application.SpecialInformations.Commands.UploadSpecialInformationFile;

internal record UploadSpecialInformationFileCommand(Guid Id, IFormFile FormFile) : ISettingsCommand<ErrorOr<Success>>;