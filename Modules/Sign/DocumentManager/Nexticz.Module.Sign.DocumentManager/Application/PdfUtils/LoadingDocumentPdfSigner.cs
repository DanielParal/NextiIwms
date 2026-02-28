using Nexticz.Lib.Shared.Math;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Lib.Shared.PdfUtils;
using Nexticz.Lib.Shared.PdfUtils.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Nexticz.Module.Sign.DocumentManager.Application.PdfUtils;

internal class LoadingDocumentPdfSigner : PdfDocumentEditor
{
    public static PdfItemPosition[] SignPdf(DocumentTemplateResponse documentTemplate, string pdfFilePath, string outputFilePath, byte[] driverSignatureFileBytes, 
        byte[] userSignatureFileBytes, string userFullName, string userDate, string userTime, string driverName,
        string driverLicensePlate, string driverDate, string driverTime)
    {
        var inputDocument = PdfSharpCore.Pdf.IO.PdfReader.Open(pdfFilePath, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Modify);
        var positionGetter = new LoadingDocumentPdfItemPositionGetter(documentTemplate);
        
        var driverSignaturePositions = positionGetter.GetDriverSignaturePositions(pdfFilePath);
        AddImage(inputDocument, driverSignaturePositions, driverSignatureFileBytes);
        
        var userSignaturePositions = positionGetter.GetUserSignaturePositions(pdfFilePath);
        AddImage(inputDocument, userSignaturePositions, userSignatureFileBytes);
        
        var textSettingsSignaturePart = new PdfTextSettings { FontSize = 10 };
        var textSettingsInnerText = new PdfTextSettings { FontSize = 9 };
        
        var userNameTextPositions = positionGetter.GetUserNameTextPositions(pdfFilePath);
        AddText(inputDocument, userNameTextPositions, userFullName, textSettingsSignaturePart);
        
        var userDateTextPositions = positionGetter.GetUserDateTextPositions(pdfFilePath);
        AddText(inputDocument, userDateTextPositions, userDate, textSettingsSignaturePart);
        
        var userTimeTextPositions = positionGetter.GetUserTimeTextPositions(pdfFilePath);
        AddText(inputDocument, userTimeTextPositions, userTime, textSettingsSignaturePart);
        
        var driverNameTextPositions = positionGetter.GetDriverNameTextPositions(pdfFilePath);
        AddText(inputDocument, driverNameTextPositions, driverName, textSettingsSignaturePart);
        
        var driverDateTextPositions = positionGetter.GetDriverDateTextPositions(pdfFilePath);
        AddText(inputDocument, driverDateTextPositions, driverDate, textSettingsSignaturePart);
        
        var driverTimeTextPositions = positionGetter.GetDriverTimeTextPositions(pdfFilePath);
        AddText(inputDocument, driverTimeTextPositions, driverTime, textSettingsSignaturePart);
        
        var driverNameDescriptionCzTextPositions = positionGetter.GetDriverNameDescriptionCzTextPositions(pdfFilePath);
        AddText(inputDocument, driverNameDescriptionCzTextPositions, driverName, textSettingsInnerText, positionGetter.GetDriverNameDescriptionCzTextBackgroundSettings());

        var driverLicensePlateDescriptionCzTextPositions = positionGetter.GetDriverLicensePlateDescriptionCzTextPositions(pdfFilePath);
        AddText(inputDocument, driverLicensePlateDescriptionCzTextPositions, driverLicensePlate, textSettingsInnerText, positionGetter.GetDriverLicensePlateCzTextBackgroundSettings());
        
        var driverNameDescriptionEnTextPositions = positionGetter.GetDriverNameDescriptionEnTextPositions(pdfFilePath);
        AddText(inputDocument, driverNameDescriptionEnTextPositions, driverName, textSettingsInnerText, positionGetter.GetDriverNameDescriptionEnTextBackgroundSettings());

        var driverLicensePlateDescriptionEnTextPositions = positionGetter.GetDriverLicensePlateDescriptionEnTextPositions(pdfFilePath);
        AddText(inputDocument, driverLicensePlateDescriptionEnTextPositions, driverLicensePlate, textSettingsInnerText, positionGetter.GetDriverLicensePlateEnTextBackgroundSettings());

        EnsureEvenPageCountForLanguageSection(inputDocument, driverSignaturePositions);
        
        var directoryName = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);
        
        inputDocument.Save(outputFilePath);
        
        return driverSignaturePositions;
    }

    private static void EnsureEvenPageCountForLanguageSection(PdfDocument inputDocument, PdfItemPosition[] driverSignaturePositions)
    {
        foreach (var position in driverSignaturePositions)
        {
            if (position.PageNumber.IsEven())
                continue;
            
            AddBlankPage(inputDocument, position.PageNumber);
        }
    }
}