using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Lib.Shared.PdfUtils;
using Nexticz.Lib.Shared.PdfUtils.Models;

namespace Nexticz.Module.Sign.DocumentManager.Application.PdfUtils;

internal class DeliveryDocumentPdfItemPositionGetter(DocumentTemplateResponse documentTemplate) : PdfPositionGetter
{
    private const string DriverSignatureTargetPhrase = "DeliveryDriverSignature";
    private const string UserSignatureTargetPhrase = "DeliveryUserSignature";
    
    private const string DriverSignature = nameof(DriverSignature);
    private const string DriverName = nameof(DriverName);
    private const string DriverDate = nameof(DriverDate);
    
    private const string UserSignature = nameof(UserSignature);
    private const string UserName = nameof(UserName);
    private const string UserDate = nameof(UserDate);
    
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
    
    public PdfItemPosition[] GetDriverNameTextPositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(DriverName);
        return GetPositions(pdfFilePath, DriverSignatureTargetPhrase, pdfItemOffset);
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
    
    public PdfItemPosition[] GetDriverDateTextPositions(string pdfFilePath)
    {
        var pdfItemOffset = GetOffsetByName(DriverDate);
        return GetPositions(pdfFilePath, DriverSignatureTargetPhrase, pdfItemOffset);
    }
    
    private PdfItemOffset GetOffsetByName(string name)
    {
        var textOffset = documentTemplate.TextOffsets.FirstOrDefault(x => x.Name == name);
        
        if (textOffset is null)
            return new PdfItemOffset(0, 0, 0, 0);
        
        return new PdfItemOffset(textOffset.Left, textOffset.Bottom, textOffset.Width, textOffset.Height);
    }
}