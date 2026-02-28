using System.Xml.Serialization;
using Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors.Models;
using Nexticz.Module.Sign.SharedKernel.FileHandling;

namespace Nexticz.Module.Sign.DocumentLoader.Application.FileProcessors;

internal static class XmlProcessor
{
    public static List<LoadingListFromXml> GetLoadingLists(string[] files)
    {
        var result = new List<LoadingListFromXml>();
        
        var loadingFiles = files.Where(x => x.EndsWith(DirectoryNamesProvider.GetLoadingListExtension));

        foreach (var file in loadingFiles)
        {
            var loadingDoc = GetLoadingList(file, files);
            
            result.Add(loadingDoc);
        }
        
        return result;
    }
    
    private static LoadingListFromXml GetLoadingList(string file, string[] files)
    {
        var loadingSerializer = new XmlSerializer(typeof(LoadingListXmlDoc));
        using var fs = new FileStream(file, FileMode.Open);
        var loadingXmlDoc = (LoadingListXmlDoc)loadingSerializer.Deserialize(fs)!;

        var loadingDoc = new LoadingListFromXml(loadingXmlDoc);

        foreach (var deliveryNoteName in loadingXmlDoc.LoadingListXml.DeliveryNotes)
        {
            var deliveryNote = GetDeliveryNote(deliveryNoteName.Name, files);
            if (deliveryNote == null) 
                continue;
            
            loadingDoc.DeliveryNotes.Add(deliveryNote);
        }
        return loadingDoc;
    }
    
    private static DeliveryNoteFromXml? GetDeliveryNote(string deliveryNoteName, string[] files)
    {
        var deliveryNoteSerializer = new XmlSerializer(typeof(DeliveryNoteXmlDoc));
        var deliveryNoteFile = files.FirstOrDefault(x => x.Contains(deliveryNoteName));
        if (deliveryNoteFile == null) 
            return null;
            
        using var fsDeliveryNote = new FileStream(deliveryNoteFile, FileMode.Open);
        var deliveryXmlDoc = (DeliveryNoteXmlDoc)deliveryNoteSerializer.Deserialize(fsDeliveryNote)!;

        return new DeliveryNoteFromXml(deliveryXmlDoc);
    }
}