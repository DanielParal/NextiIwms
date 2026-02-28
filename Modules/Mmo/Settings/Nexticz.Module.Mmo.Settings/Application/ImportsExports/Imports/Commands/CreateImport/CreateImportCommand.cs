using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Nexticz.Module.Mmo.Settings.Domain.ImportEntity;

namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Imports.Commands.CreateImport;

internal record CreateImportCommand(string UserName, ImportType ImportType, IFormFile FormFile) : ISettingsCommand<ErrorOr<Import>>;