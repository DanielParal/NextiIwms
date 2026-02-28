using PdfSharpCore.Drawing;

namespace Nexticz.Lib.Shared.PdfUtils.Models;

public class PdfTextSettings
{
    public string FontName { get; set; } = "Arial";
    public double FontSize { get; set; } = 12;
    public XFontStyle FontStyle { get; set; } = XFontStyle.Regular;
    public XBrush Brush { get; set; } = XBrushes.Black;
    public XStringFormat StringFormat { get; set; } = XStringFormats.TopLeft;
}