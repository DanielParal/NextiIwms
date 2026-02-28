using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.DocumentLoader.Contracts.Notifications;
using Nexticz.Module.Sign.DocumentLoader.Application.FileHandling;
using Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors;
using Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Models;
using Nexticz.Module.Sign.DocumentLoader.Application.Interfaces;
using Nexticz.Module.Sign.DocumentLoader.Application.NotificationCollectors;
using Nexticz.Module.Sign.DocumentLoader.Domain.LoadingDocumentAggregate;
using Nexticz.Module.Sign.DocumentLoader.Domain.LoadingDocumentAggregate.Events;

namespace Nexticz.Module.Sign.DocumentLoader.Application.LoadingDocuments.Commands.ProcessXmlFiles;

internal class ProcessXmlFilesCommandHandler(
    ILogger<ProcessXmlFilesCommandHandler> logger,
    IDocumentLoaderFileHandler fileHandler,
    IDocumentLoaderNotificationCollector notificationCollector,
    IDocumentLoaderUnitOfWork unitOfWork) : IRequestHandler<ProcessXmlFilesCommand, Success>
{
    public Task<Success> Handle(ProcessXmlFilesCommand request, CancellationToken cancellationToken)
    {
        var loadingListsFromXml = ProcessXmlFiles();

        foreach (var loadingListFromXml in loadingListsFromXml)
        {
            if (!ValidateAllFilesAreCopiedOverFromFtp(request.RoundKey, loadingListFromXml))
                continue;
            
            var loadingDocumentAggregate = 
                new LoadingDocument(
                    loadingListFromXml.Code, 
                    loadingListFromXml.DeliveryNotes.Select(x => x.Code).ToArray());
            
            var loadingListProcessedEvent =
                new LoadingDocumentXmlProcessedEvent(loadingDocumentAggregate.Id, loadingDocumentAggregate.Code, loadingDocumentAggregate.DeliveryDocumentCodes);

            unitOfWork.StartStream<LoadingDocumentXmlProcessedEvent, LoadingDocument>(
                loadingDocumentAggregate.Id, loadingListProcessedEvent);
            
            logger.LogInformation("SIGN - RoundKey: {roundKey} - loading document with code: {LoadingListCode} and with number of delivery documents: {DeliveryNotesCount} was processed.", 
                request.RoundKey, loadingDocumentAggregate.Code, loadingDocumentAggregate.DeliveryDocumentCodes.Length);

            var loadingDocumentContract = LoadingDocumentContractFactory.Create(loadingListFromXml);
            notificationCollector.AddNotification(new LoadingDocumentXmlProcessedNotification(loadingDocumentContract));
        }
        
        return Task.FromResult(Result.Success);
    }
    
    private List<LoadingListFromXml> ProcessXmlFiles()
    {
        var files = fileHandler.GetXmlFilesToLoad();
        
        return files.Length == 0 ? [] : XmlProcessor.GetLoadingLists(files);
    }
    
    private bool ValidateAllFilesAreCopiedOverFromFtp(string roundKey, LoadingListFromXml loadingListFromXml)
    {
        var files = fileHandler.GetXmlFilesToLoad(loadingListFromXml.Code + "*.xml");

        if (files.Length == 0)
        {
            logger.LogWarning("SIGN - RoundKey: {roundKey} - no xml files for loading list: {LoadingListCode} were found on disk.", 
                roundKey, loadingListFromXml.Code);
            return false;
        }
            
        
        var fileCountFromXml = loadingListFromXml.TotalDeliveryNotes + 1;

        if (fileCountFromXml != files.Length)
        {
            logger.LogWarning("SIGN - RoundKey: {roundKey} - not all xml files for loading list: {LoadingListCode} are copied over from ftp. Xml fiiles count in xml: {FilesCountInXml}, Xml files count on disk: {FilesCountOnDisk}.", 
                roundKey, loadingListFromXml.Code, fileCountFromXml, files.Length);
            return false;
        }

        if (!fileHandler.DoesFileExist($"{loadingListFromXml.Code}.pdf"))
        {
            logger.LogWarning("SIGN - RoundKey: {roundKey} - loading document pdf is not copied over from ftp, Loading document code: {LoadingListCode}", 
                roundKey, loadingListFromXml.Code);
            return false;
        }

        foreach (var deliveryNote in loadingListFromXml.DeliveryNotes)
        {
            if (fileHandler.DoesFileExist($"{deliveryNote.Code}.pdf")) 
                continue;
            
            logger.LogWarning("SIGN - RoundKey: {roundKey} - delivery document pdf is not copied over from ftp, Loading document code: {LoadingListCode}, Delivery document code: {DeliveryNoteCode}", 
                roundKey, loadingListFromXml.Code, deliveryNote.Code);
            return false;
        }
            
        return true;
    }
}