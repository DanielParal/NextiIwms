using System.Text;
using Nexticz.Lib.Shared.PdfUtils.Models;
using UglyToad.PdfPig;

namespace Nexticz.Lib.Shared.PdfUtils;

public abstract class PdfPositionGetter
{
    protected static PdfItemPosition[] GetPositions(string pdfFilePath, string targetPhrase, PdfItemOffset offset)
    {
        using var document = PdfDocument.Open(pdfFilePath);

        var target = targetPhrase.Normalize(NormalizationForm.FormD);
        var results = new List<PdfItemPosition>();
        
        foreach (var page in document.GetPages())
        {
            var words = page.GetWords();

            foreach (var word in words)
            {
                var text = word.Text.Normalize(NormalizationForm.FormD);
                if (!string.Equals(text, target, StringComparison.OrdinalIgnoreCase)) 
                    continue;
                
                var bbox = word.BoundingBox;
                results.Add(
                    new PdfItemPosition(
                        page.Number, 
                        bbox.Left + 
                        offset.Left, 
                        bbox.Bottom + offset.Bottom, 
                        bbox.Width + offset.Width, 
                        bbox.Height + offset.Height, 
                        page.Rotation.Value));
            }
        }
        
        return results.ToArray();
    }
}