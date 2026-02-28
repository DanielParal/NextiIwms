namespace Nexticz.Module.Cuzk.Application.ImportsExports.Imports.Orchestrators;

internal interface IImportProcessorOrchestrator
{
    Task ProcessImportsAsync(string roundKey, CancellationToken cancellationToken);
}