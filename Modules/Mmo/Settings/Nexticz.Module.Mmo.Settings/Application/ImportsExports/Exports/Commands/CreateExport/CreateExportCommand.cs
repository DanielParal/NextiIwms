using ErrorOr;
using MediatR;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Mmo.Settings.Domain.ExportEntity;

namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports.Exports.Commands.CreateExport;

internal record CreateExportCommand(string UserName, ExportType ExportType) : ISettingsCommand<ErrorOr<FileResult>>;