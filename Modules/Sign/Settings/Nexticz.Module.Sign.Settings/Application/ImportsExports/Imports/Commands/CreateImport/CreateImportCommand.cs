using ErrorOr;
using Microsoft.AspNetCore.Http;
using Nexticz.Module.Sign.Settings.Domain.ImportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Imports.Commands.CreateImport;

internal record CreateImportCommand(ImportType ImportType, IFormFile FormFile) : ISettingsCommand<ErrorOr<Import>>;