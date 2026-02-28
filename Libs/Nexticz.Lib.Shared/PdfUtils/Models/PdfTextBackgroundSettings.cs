using PdfSharpCore.Drawing;

namespace Nexticz.Lib.Shared.PdfUtils.Models;

public class PdfTextBackgroundSettings
{
    public double XPositionOffset { get; set; } = 0;
    public double Width { get; set; } = 0;
    public XBrush Brush { get; set; } = XBrushes.White;
}