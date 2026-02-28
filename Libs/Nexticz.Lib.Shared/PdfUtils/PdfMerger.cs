using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace Nexticz.Lib.Shared.PdfUtils;

public class PdfMerger
{
    public static byte[] MergePdfs(string[] inputPdfPaths)
    {
        using var outputDocument = new PdfDocument();
        using var memoryStream = new MemoryStream();
        foreach (var pdfPath in inputPdfPaths)
        {
            using var inputDocument = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
            
            if (inputDocument is null || inputDocument.PageCount == 0)
                continue;
            
            for (var i = 0; i < inputDocument.PageCount; i++)
            {
                var page = inputDocument.Pages[i];
                outputDocument.AddPage(page);
            }
        }
        outputDocument.Save(memoryStream, false);
        return memoryStream.ToArray();
    }
    
    public static byte[] MergePdfsAndNormalizedToPortraitForPrinting(string[] inputPdfPaths, bool eachDocOnSeparateSheet)
    {
        using var outputDocument = new PdfDocument();
        using var memoryStream = new MemoryStream();

        foreach (var pdfPath in inputPdfPaths)
        {
            NormalizeLandscapeToPortrait(outputDocument, pdfPath, eachDocOnSeparateSheet);
        }

        outputDocument.Save(memoryStream, false);
        return memoryStream.ToArray();
    }
    
    private static void NormalizeLandscapeToPortrait(PdfDocument outputDocument, string pdfPath, bool eachDocOnSeparateSheet)
    {
        var form = XPdfForm.FromFile(pdfPath);

        var pagesAddedForThisDoc = 0;
        XSize lastOutSize = default;
        
        for (var i = 1; i <= form.PageCount; i++)
        {
            form.PageNumber = i;

            var srcW = form.PointWidth;
            var srcH = form.PointHeight;

            var outPage = outputDocument.AddPage();

            var isLandscapeContent = srcW > srcH;
            SetPageSizes(outPage, isLandscapeContent, srcW, srcH);
            DrawPage(form, outPage, isLandscapeContent);
            
            lastOutSize = new XSize(outPage.Width, outPage.Height);
            pagesAddedForThisDoc++;
        }
        
        if (ShouldAddBlankPageToHaveEvenNumberOfPagesForDocument(eachDocOnSeparateSheet, pagesAddedForThisDoc))
        {
            AddBlankPage(outputDocument, lastOutSize.Width, lastOutSize.Height);
        }
    }

    private static void SetPageSizes(PdfPage page, bool isLandscape, double srcWidth, double srcHeight)
    {
        if (isLandscape)
        {
            page.Width = srcHeight;
            page.Height = srcWidth;
            return;
        }
        
        page.Width = srcWidth;
        page.Height = srcHeight;
    }

    private static void DrawPage(XPdfForm form, PdfPage page, bool isLandscape)
    {
        using var gfx = XGraphics.FromPdfPage(page);
        if (isLandscape)
        {
            // Draw rotated into portrait page
            gfx.TranslateTransform(0, page.Height);
            gfx.RotateTransform(-90);
            gfx.DrawImage(form, 0, 0, page.Height, page.Width);
            return;
        }
        
        // Draw as-is
        gfx.DrawImage(form, 0, 0, page.Width, page.Height);
    }

    private static bool ShouldAddBlankPageToHaveEvenNumberOfPagesForDocument(bool eachDocOnSeparateSheet, int pagesAddedForThisDoc)
    {
        return eachDocOnSeparateSheet && pagesAddedForThisDoc % 2 != 0;
    }
    
    private static void AddBlankPage(PdfDocument outputDocument, double width, double height)
    {
        var blank = outputDocument.AddPage();
        blank.Width = width;
        blank.Height = height;
    }
}