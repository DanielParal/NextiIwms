using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Lib.Shared.PdfUtils;
using Nexticz.Lib.Shared.PdfUtils.Models;

namespace Nexticz.Module.Sign.DocumentManager.Application.PdfUtils;

internal class DeliveryDocumentPdfSigner : PdfDocumentEditor
{
    public static PdfItemPosition[] SignPdf(DocumentTemplateResponse documentTemplate, string pdfFilePath, string outputFilePath, byte[] driverSignatureFileBytes, 
        byte[] userSignatureFileBytes, string userFullName, string driverName, string currentDate)
    {
        var inputDocument = PdfSharpCore.Pdf.IO.PdfReader.Open(pdfFilePath, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Modify);
        var positionGetter = new DeliveryDocumentPdfItemPositionGetter(documentTemplate);
        
        var driverSignaturePositions = positionGetter.GetDriverSignaturePositions(pdfFilePath);
        AddImage(inputDocument, driverSignaturePositions, driverSignatureFileBytes);
        
        var userSignaturePositions = positionGetter.GetUserSignaturePositions(pdfFilePath);
        AddImage(inputDocument, userSignaturePositions, userSignatureFileBytes);
        
        var driverNameTextPositions = positionGetter.GetDriverNameTextPositions(pdfFilePath);
        AddText(inputDocument, driverNameTextPositions, driverName);
        
        var userNameTextPositions = positionGetter.GetUserNameTextPositions(pdfFilePath);
        AddText(inputDocument, userNameTextPositions, userFullName);
        
        var userDateTextPositions = positionGetter.GetUserDateTextPositions(pdfFilePath);
        AddText(inputDocument, userDateTextPositions, currentDate);
        
        var driverDateTextPositions = positionGetter.GetDriverDateTextPositions(pdfFilePath);
        AddText(inputDocument, driverDateTextPositions, currentDate);
    
        var directoryName = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrWhiteSpace(directoryName) && Directory.Exists(directoryName) == false)
            Directory.CreateDirectory(directoryName);
        
        inputDocument.Save(outputFilePath);
        
        return driverSignaturePositions;
    }
}