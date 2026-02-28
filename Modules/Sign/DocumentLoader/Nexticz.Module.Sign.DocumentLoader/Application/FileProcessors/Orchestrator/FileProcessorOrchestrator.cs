using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentLoader.Application.FileHandling;
using Nexticz.Module.Sign.DocumentLoader.Application.LoadingDocuments.Commands.ProcessXmlFiles;

namespace Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Orchestrator;

internal class FileProcessorOrchestrator(
    ILogger<FileProcessorOrchestrator> logger,
    IDocumentLoaderFileHandler fileHandler,
    ISender sender) : IFileProcessorOrchestrator
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);
    public async Task ProcessFilesAsync(string roundKey, CancellationToken cancellationToken)
    {
        // Only one process can run at a time. Currently, we can call it only from background worker,
        // but in the future we can also call it from api which can cause race conditions.
        if (!await Semaphore.WaitAsync(TimeSpan.Zero, cancellationToken))
        {
            logger.LogInformation("SIGN - RoundKey: {roundKey} - Processing is already running.", roundKey);
            return;
        }

        try
        {
            await CopyFilesFromFtpAsync(roundKey, cancellationToken);
            await sender.Send(new ProcessXmlFilesCommand(roundKey), cancellationToken);
        }
        finally
        {
            Semaphore.Release();
        }
    }

    private async Task CopyFilesFromFtpAsync(string roundKey, CancellationToken cancellationToken)
    {
        var files = await fileHandler.ListVerifiedFtpFilesAsync(cancellationToken);

        if (files.Length > 0)
        {
            await fileHandler.CopyFileFromFtpToLoaderFolderAsync(roundKey, files, cancellationToken);
        }
        else
        {
            logger.LogDebug("SIGN - RoundKey: {roundKey} - No files found in FTP folder.", roundKey);
        }
    }
}