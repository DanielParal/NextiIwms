using Nexticz.Lib.Shared.PdfUtils.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Nexticz.Lib.Shared.PdfUtils;

public abstract class PdfDocumentEditor
{
    protected static PdfDocument AddImage(PdfDocument inputDocument, PdfItemPosition[] positions, byte[] imageBytesToInsert, bool shouldTakeProportionalWidth = true)
    {
        using var image = XImage.FromStream(() => new MemoryStream(imageBytesToInsert));
        foreach (var position in positions)
        {
            var page = inputDocument.Pages[position.PageNumber - 1];
            using var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
            var yFromTop = page.Height - position.Bottom - position.Height;
            
            if (position.PageRotation != 0)
                RotateXGraphics(gfx, position.PageRotation, page.Height);
            
            var width = shouldTakeProportionalWidth ? GetProportionalWidth(image, position.Height) : position.Width;
            gfx.DrawImage(image, position.Left, yFromTop, width, position.Height);
        }

        return inputDocument;
    }
    
    protected static PdfDocument AddText(PdfDocument inputDocument, PdfItemPosition[] positions, string text, 
        PdfTextSettings? settings = null, PdfTextBackgroundSettings? textBackgroundSettings = null)
    {
        settings ??= new PdfTextSettings();
        
        foreach (var position in positions)
        {
            var page = inputDocument.Pages[position.PageNumber - 1];
            
            using var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
        
            var font = new XFont(settings.FontName, settings.FontSize, settings.FontStyle);
        
            var x = position.Left;
            var yFromTop = page.Height - position.Bottom - position.Height;;
        
            if (position.PageRotation != 0)
                RotateXGraphics(gfx, position.PageRotation, page.Height);
            
            if (textBackgroundSettings is not null)
            {
                // Measure text size to know how big the white background should be
                var textSize = gfx.MeasureString(text, font);
                // Draw a white rectangle behind the text
                var backgroundRect = new XRect(x + textBackgroundSettings.XPositionOffset, yFromTop, textSize.Width + textBackgroundSettings.Width, textSize.Height);
                gfx.DrawRectangle(textBackgroundSettings.Brush, backgroundRect);
            }
            
            gfx.DrawString(text, font, settings.Brush, new XPoint(x, yFromTop), settings.StringFormat);
        }
        
        return inputDocument;
    }
    
    protected static void AddBlankPage(PdfDocument inputDocument, int pageNumberInsertAfter)
    {
        var insertAfterIndex = pageNumberInsertAfter - 1; // 0-based index

        var template = inputDocument.Pages[insertAfterIndex];
        var blank = new PdfPage
        {
            Width = template.Width,
            Height = template.Height
        };
        
        inputDocument.Pages.Insert(pageNumberInsertAfter, blank);
    }
    
    private static double GetProportionalWidth(XImage image, double positionHeight)
    {
        var imageRation = image.PixelWidth / (double)image.PixelHeight;
        return positionHeight * imageRation;
    }
    
    private static void RotateXGraphics(XGraphics gfx, int degrees, XUnit pageHeight)
    {
        gfx.RotateTransform(-degrees);
        gfx.TranslateTransform(-pageHeight, 0);
    }
}