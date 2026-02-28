using Microsoft.Playwright;
using Nexticz.Lib.Shared.PdfUtils.Models;

namespace Nexticz.Lib.Shared.PdfUtils;

public abstract class PdfGenerator
{
    public static async Task<byte[]> HtmlToPdfAsync(string html, PrinterPageSize pageSize)
    {
        // 1. Initialize Playwright & launch headless Chromium
        using var playwright = await Playwright.CreateAsync();
        await using var browser =
            await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

        // 2. Create a new page and set your HTML
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html, new PageSetContentOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        // 3. Generate the PDF (byte[])
        var pdfBytes = await page.PdfAsync(new PagePdfOptions
        {
            Format = pageSize.ToString(),
            PrintBackground = true,
            Landscape = true,
            Margin = new Margin
            {
                Top = "1mm",
                Bottom = "1mm",
                Left = "1mm",
                Right = "1mm"
            }
        });

        // 4. Clean up and return
        await browser.CloseAsync();
        return pdfBytes;
    }
}