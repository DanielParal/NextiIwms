using ErrorOr;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Sign.Settings.Domain.ExportAggregate;

namespace Nexticz.Module.Sign.Settings.Application.ImportsExports.Exports.Commands.CreateExport;

internal record CreateExportCommand(string UserName, ExportType ExportType) : ISettingsCommand<ErrorOr<FileResult>>;