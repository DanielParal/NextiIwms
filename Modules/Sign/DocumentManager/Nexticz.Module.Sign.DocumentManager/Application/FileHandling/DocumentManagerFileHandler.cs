using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.SharedKernel.FileHandling;
using Nexticz.Lib.Shared.FileHandling;
using Nexticz.Lib.Shared.FileHandling.Assets;
using Nexticz.Lib.Shared.FileHandling.Models;
using Nexticz.Module.Sign.DocumentManager.Domain.LoadingDocumentAggregate;

namespace Nexticz.Module.Sign.DocumentManager.Application.FileHandling;

internal class DocumentManagerFileHandler(
    ILogger<DocumentManagerFileHandler> logger,
    AssetsSettings assetsSettings) : FileHandler(logger), IDocumentManagerFileHandler
{
    public async Task CopyLoadingListFromLoaderFolderToManagerFolderAsync(LoadingDocument loadingDocument, CancellationToken cancellationToken)
    {
        var loaderFolderPath = DirectoryNamesProvider.GetBaseLoaderPath(assetsSettings);
        var managerFolderPath = DirectoryNamesProvider.GetBaseManagerPath(assetsSettings);
        var destinationManagerFolderPath = Path.Combine(managerFolderPath, loadingDocument.Code);

        try
        {
            var loadingListXmlFileName = DirectoryNamesProvider.GetLoadingListXmlFileName(loadingDocument.Code);
            
            await CopyFileFromSourceToDestinationAsync(
                loadingListXmlFileName, $"{loadingDocument.Code}.xml", loaderFolderPath, destinationManagerFolderPath, true, false, cancellationToken);

            logger.LogInformation("SIGN - DocumentManager - Copied file {FileName} to {DestinationPath}", 
                loadingListXmlFileName, destinationManagerFolderPath);
            
            var loadingListPdfFileName = DirectoryNamesProvider.GetLoadingListPdfFileName(loadingDocument.Code);
            
            await CopyFileFromSourceToDestinationAsync(
                loadingListPdfFileName, $"{loadingDocument.Code}.pdf", loaderFolderPath, destinationManagerFolderPath, true, false, cancellationToken);

            logger.LogInformation("SIGN - DocumentManager - Copied file {FileName} to {DestinationPath}", 
                loadingListPdfFileName, destinationManagerFolderPath);
            
            foreach (var deliveryNoteCode in loadingDocument.DeliveryDocuments)
            {
                var deliveryNoteXmlFileName =
                    DirectoryNamesProvider.GetDeliveryNoteXmlFileName(loadingDocument.Code, deliveryNoteCode.Code);
                
                await CopyFileFromSourceToDestinationAsync(
                    deliveryNoteXmlFileName, $"{deliveryNoteCode.Code}.xml", loaderFolderPath, destinationManagerFolderPath, true, false, cancellationToken);

                logger.LogInformation("SIGN - DocumentManager - Copied file {FileName} to {DestinationPath}", 
                    deliveryNoteXmlFileName, destinationManagerFolderPath);
                
                var deliveryNotePdfFileName =
                    DirectoryNamesProvider.GetDeliveryNotePdfFileName(deliveryNoteCode.Code);
                
                await CopyFileFromSourceToDestinationAsync(
                    deliveryNotePdfFileName, $"{deliveryNoteCode.Code}.pdf", loaderFolderPath, destinationManagerFolderPath, true, false, cancellationToken);

                logger.LogInformation("SIGN - DocumentManager - Copied file {FileName} to {DestinationPath}", 
                    deliveryNotePdfFileName, destinationManagerFolderPath);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SIGN - DocumentManager - Error copying files from loader path: {LoaderPath} to manager path {ManagerPath}. ErrorMessage: {ErrorMessage}", 
                loaderFolderPath, managerFolderPath, ex.Message);
            throw;
        }
    }

    public async Task<FileResult?> GetDocumentFileFromManagerAsync(string loadingDocumentCode, string documentFileName, CancellationToken cancellationToken)
    {
        var loadingDocumentFolderPath = $"{DirectoryNamesProvider.GetBaseManagerPath(assetsSettings)}/{loadingDocumentCode}";
        return await GetFileAsync(loadingDocumentFolderPath, documentFileName, cancellationToken);
    }
    
    public async Task<FileResult?> GetDocumentFileFromHistoryAsync(string loadingDocumentCode, string? deliveryDocumentCode, CancellationToken cancellationToken)
    {
        var fileName = string.IsNullOrEmpty(deliveryDocumentCode) ? 
            $"{loadingDocumentCode}.pdf" : $"{deliveryDocumentCode}.pdf";
        
        var loadingDocumentFolderPath = $"{DirectoryNamesProvider.GetBaseHistoryPath(assetsSettings)}/{loadingDocumentCode}";
        return await GetFileAsync(loadingDocumentFolderPath, fileName, cancellationToken);
    }

    public async Task CopyDocumentFilesFromManagerToHistorySourceFilesFolderAsync(string loadingDocumentCode, string? deliveryDocumentCode,
        CancellationToken cancellationToken)
    {
        var managerLoadingDocumentFolderPath = Path.Combine(DirectoryNamesProvider.GetBaseManagerPath(assetsSettings), loadingDocumentCode);
        var historyOriginalFilesPath =
            DirectoryNamesProvider.GetHistoryLoadingDocumentOriginalFilesPath(assetsSettings, loadingDocumentCode);

        if (string.IsNullOrWhiteSpace(deliveryDocumentCode))
        {
            await CopyFileFromSourceToDestinationAsync(
                $"{loadingDocumentCode}.pdf", $"{loadingDocumentCode}.pdf", managerLoadingDocumentFolderPath, historyOriginalFilesPath, true, true, cancellationToken);
            
            await CopyFileFromSourceToDestinationAsync(
                $"{loadingDocumentCode}.xml", $"{loadingDocumentCode}.xml", managerLoadingDocumentFolderPath, historyOriginalFilesPath, true,  true,cancellationToken);

            return;
        }
        
        await CopyFileFromSourceToDestinationAsync(
            $"{deliveryDocumentCode}.pdf", $"{deliveryDocumentCode}.pdf", managerLoadingDocumentFolderPath, historyOriginalFilesPath, true, true, cancellationToken);
            
        await CopyFileFromSourceToDestinationAsync(
            $"{deliveryDocumentCode}.xml", $"{deliveryDocumentCode}.xml", managerLoadingDocumentFolderPath, historyOriginalFilesPath, true, true, cancellationToken);
    }

    public async Task RevertDocumentFilesFromHistorySourceFilesToManagerFolderAsync(string loadingDocumentCode, string? deliveryDocumentCode,
        CancellationToken cancellationToken)
    {
        var managerLoadingDocumentFolderPath = Path.Combine(DirectoryNamesProvider.GetBaseManagerPath(assetsSettings), loadingDocumentCode);
        var historyOriginalSourceFilesPath = DirectoryNamesProvider.GetHistoryLoadingDocumentOriginalFilesPath(assetsSettings, loadingDocumentCode);
        var historyLoadingDocumentFolderPath = Path.Combine(DirectoryNamesProvider.GetBaseHistoryPath(assetsSettings), loadingDocumentCode);

        if (string.IsNullOrWhiteSpace(deliveryDocumentCode))
        {
            await CopyFileFromSourceToDestinationAsync(
                $"{loadingDocumentCode}.pdf", $"{loadingDocumentCode}.pdf", historyOriginalSourceFilesPath, managerLoadingDocumentFolderPath, true, true, cancellationToken);
            
            await CopyFileFromSourceToDestinationAsync(
                $"{loadingDocumentCode}.xml", $"{loadingDocumentCode}.xml", historyOriginalSourceFilesPath, managerLoadingDocumentFolderPath, true, true, cancellationToken);

            DeleteFile(historyLoadingDocumentFolderPath, $"{loadingDocumentCode}.pdf");
            
            return;
        }
        
        await CopyFileFromSourceToDestinationAsync(
            $"{deliveryDocumentCode}.pdf", $"{deliveryDocumentCode}.pdf", historyOriginalSourceFilesPath, managerLoadingDocumentFolderPath, true, true, cancellationToken);
            
        await CopyFileFromSourceToDestinationAsync(
            $"{deliveryDocumentCode}.xml", $"{deliveryDocumentCode}.xml", historyOriginalSourceFilesPath, managerLoadingDocumentFolderPath, true, true, cancellationToken);
        
        DeleteFile(historyLoadingDocumentFolderPath, $"{deliveryDocumentCode}.pdf");
    }

    public async Task SaveSignedDocumentAsync(IFormFile formFile, string loadingDocumentCode, string? deliveryDocumentCode,
        CancellationToken cancellationToken)
    {
        var historyLoadingDocumentFolderPath = Path.Combine(DirectoryNamesProvider.GetBaseHistoryPath(assetsSettings), loadingDocumentCode);
        var fileName = deliveryDocumentCode is null ? $"{loadingDocumentCode}.pdf" : $"{deliveryDocumentCode}.pdf";
        await SaveFileAsync(formFile, historyLoadingDocumentFolderPath, fileName, cancellationToken);
    }
}