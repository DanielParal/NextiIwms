using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Lib.Shared.PdfUtils;
using Nexticz.Lib.Shared.PdfUtils.Models;
using PdfSharpCore.Drawing;

namespace Nexticz.Module.Sign.DocumentManager.Application.PdfUtils;

internal class LoadingDocumentPdfItemPositionGetter(DocumentTemplateResponse documentTemplate) : PdfPositionGetter
{
    private const string DriverSignatureTargetPhrase = "LoadingDriverSignature";
    private const string UserSignatureTargetPhrase = "LoadingUserSignature";

    private const string DriverSignature = nameof(DriverSignature);
    private const string DriverName = nameof(DriverName);
    private const string DriverDate = nameof(DriverDate);
    private const string DriverTime = nameof(DriverTime);
    
    private const string UserSignature = nameof(UserSignature);
    private const string UserName = nameof(UserName);
    private const string UserDate = nameof(UserDate);
    private const string UserTime = nameof(UserTime);
    
    private const string DriverNameDescriptionCz = nameof(DriverNameDescriptionCz);
    private const string DriverNameDescriptionEn = nameof(DriverNameDescriptionEn);
    private const string DriverLicensePlateDescriptionCz = nameof(DriverLicensePlateDescriptionCz);
    private const string DriverLicensePlateDescriptionEn = nameof(DriverLicensePlateDescriptionEn);
    
    private const string DriverNameDescriptionCzBackground = nameof(DriverNameDescriptionCz);
    private const string DriverNameDescriptionEnBackground = nameof(DriverNameDescriptionEn);
    private const string DriverLicensePlateDescriptionCzBackground = nameof(DriverLicensePlateDescriptionCz);
    private const string DriverLicensePlateDescriptionEnBackground = nameof(DriverLicensePlateDescriptionEn);

    public PdfItemPosition[] GetDriverSignaturePositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(DriverSignature);
        return GetPositions(pdfFilePath, DriverSignatureTargetPhrase, pdfItemOffset);
    }
    
    public PdfItemPosition[] GetUserSignaturePositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(UserSignature);
        return GetPositions(pdfFilePath, UserSignatureTargetPhrase, pdfItemOffset);
    }
    
    public PdfItemPosition[] GetUserNameTextPositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(UserName);
        return GetPositions(pdfFilePath, UserSignatureTargetPhrase, pdfItemOffset);
    }
    
    public PdfItemPosition[] GetUserDateTextPositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(UserDate);
        return GetPositions(pdfFilePath, UserSignatureTargetPhrase, pdfItemOffset);
    }
    
    public PdfItemPosition[] GetUserTimeTextPositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(UserTime);
        return GetPositions(pdfFilePath, UserSignatureTargetPhrase, pdfItemOffset);
    }
    
    public PdfItemPosition[] GetDriverNameTextPositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(DriverName);
        return GetPositions(pdfFilePath, DriverSignatureTargetPhrase, pdfItemOffset);
    }
    
    public PdfItemPosition[] GetDriverDateTextPositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(DriverDate);
        return GetPositions(pdfFilePath, DriverSignatureTargetPhrase, pdfItemOffset);
    }
    
    public PdfItemPosition[] GetDriverTimeTextPositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(DriverTime);
        return GetPositions(pdfFilePath, DriverSignatureTargetPhrase, pdfItemOffset);
    }
    
    public PdfItemPosition[] GetDriverNameDescriptionCzTextPositions(string pdfFilePath)
    {
        const string driverNameDescriptionTargetPhraseCzech = "Řidič:";
        var pdfItemOffset = GetOffsetByName(DriverNameDescriptionCz);
        var czechPositions = GetPositions(pdfFilePath, driverNameDescriptionTargetPhraseCzech, pdfItemOffset);
        return czechPositions;
    }
    
    public PdfItemPosition[] GetDriverNameDescriptionEnTextPositions(string pdfFilePath)
    {
        const string driverNameDescriptionTargetPhraseEnglish = "Driver:";
        var pdfItemOffset = GetOffsetByName(DriverNameDescriptionEn);
        var englishPositions = GetPositions(pdfFilePath, driverNameDescriptionTargetPhraseEnglish, pdfItemOffset);
        return englishPositions;
    }
    
    public PdfItemPosition[] GetDriverLicensePlateDescriptionCzTextPositions(string pdfFilePath)
    {
        const string driverSpzDescriptionTargetPhraseCzech = "Vozidla:";
        var pdfItemOffset = GetOffsetByName(DriverLicensePlateDescriptionCz);
        var czechPositions = GetPositions(pdfFilePath, driverSpzDescriptionTargetPhraseCzech, pdfItemOffset);
        return czechPositions;
    }
    
    public PdfItemPosition[] GetDriverLicensePlateDescriptionEnTextPositions(string pdfFilePath)
    {
        const string driverSpzDescriptionTargetPhraseEnglish = "plate:";
        var pdfItemOffset = GetOffsetByName(DriverLicensePlateDescriptionEn);
        var englishPositions = GetPositions(pdfFilePath, driverSpzDescriptionTargetPhraseEnglish, pdfItemOffset);
        return englishPositions;
    }

    public PdfTextBackgroundSettings GetDriverNameDescriptionCzTextBackgroundSettings()
    {
        var textBackground = GetBackgroundByName(DriverNameDescriptionCzBackground);
        return new PdfTextBackgroundSettings { Brush = XBrushes.White, XPositionOffset = textBackground.XPositionOffset, Width = textBackground.Width };
    }
    
    public PdfTextBackgroundSettings GetDriverNameDescriptionEnTextBackgroundSettings()
    {
        var textBackground = GetBackgroundByName(DriverNameDescriptionEnBackground);
        return new PdfTextBackgroundSettings { Brush = XBrushes.White, XPositionOffset = textBackground.XPositionOffset, Width = textBackground.Width };
    }
    
    public PdfTextBackgroundSettings GetDriverLicensePlateCzTextBackgroundSettings()
    {
        var textBackground = GetBackgroundByName(DriverLicensePlateDescriptionCzBackground);
        return new PdfTextBackgroundSettings { Brush = XBrushes.White, XPositionOffset = textBackground.XPositionOffset, Width = textBackground.Width };
    }
    
    public PdfTextBackgroundSettings GetDriverLicensePlateEnTextBackgroundSettings()
    {
        var textBackground = GetBackgroundByName(DriverLicensePlateDescriptionEnBackground);
        return new PdfTextBackgroundSettings { Brush = XBrushes.White, XPositionOffset = textBackground.XPositionOffset, Width = textBackground.Width };
    }

    private PdfTextBackgroundSettings GetBackgroundByName(string name)
    {
        var textBackground = documentTemplate.TextBackgrounds.FirstOrDefault(x => x.Name == name);
        
        if (textBackground is null)
            return new PdfTextBackgroundSettings { Brush = XBrushes.White, XPositionOffset = -1, Width = 50 };
        
        return new PdfTextBackgroundSettings { Brush = XBrushes.White, XPositionOffset = textBackground.XPositionOffset, Width = textBackground.Width };
    }

    private PdfItemOffset GetOffsetByName(string name)
    {
        var textOffset = documentTemplate.TextOffsets.FirstOrDefault(x => x.Name == name);
        
        if (textOffset is null)
            return new PdfItemOffset(0, 0, 0, 0);
        
        return new PdfItemOffset(textOffset.Left, textOffset.Bottom, textOffset.Width, textOffset.Height);
    }
}