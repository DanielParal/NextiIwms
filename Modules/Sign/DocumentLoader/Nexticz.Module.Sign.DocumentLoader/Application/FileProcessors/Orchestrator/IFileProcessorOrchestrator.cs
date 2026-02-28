namespace Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Orchestrator;

internal interface IFileProcessorOrchestrator
{
    Task ProcessFilesAsync(string roundKey, CancellationToken cancellationToken);
}