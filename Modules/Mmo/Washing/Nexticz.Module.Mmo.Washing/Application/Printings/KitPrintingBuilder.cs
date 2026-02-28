using Nexticz.Lib.Shared.PdfUtils;
using Nexticz.Lib.Shared.PdfUtils.Models;

namespace Nexticz.Module.Mmo.Washing.Application.Printings;

internal static class KitPrintingBuilder
{
    public static async Task<PrintingFile> BuildKitPrintingPdfAsync(string fileName, Dictionary<string, string> data, PrinterPageSize pageSize)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Application/Printings/Templates", "KitPrintoutTemplate.html");
        var html = await File.ReadAllTextAsync(filePath);

        foreach (var (key, value) in data)
        {
            html = html.Replace("{{" + key + "}}", value);
        }

        return new PrintingFile(
            await PdfGenerator.HtmlToPdfAsync(html, pageSize), "application/pdf", fileName);
    }
}