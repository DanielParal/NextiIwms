
using  BarcodeStandard;
using Microsoft.Extensions.Logging;
using Type = BarcodeStandard.Type;

namespace Nexticz.Module.Mmo.Washing.Application.Printings;

internal class BarcodeGenerator(ILogger<BarcodeGenerator> logger)
{
    public string? GetBarcode(string barcodeText)
    {
        try
        {
            using var barcode = new Barcode(barcodeText, Type.Code39);
            using var barcodeImage = barcode.Encode(Type.Code39, barcodeText, 400, 200);
            var bytes = barcode.EncodedImageBytes;
            var base64 = Convert.ToBase64String(bytes);

            return $"data:image/png;base64,{base64}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "MMO - Washing - Error generating barcode.");
            return null;
        }
    }
}