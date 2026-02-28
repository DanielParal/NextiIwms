using System.Drawing;
using QRCoder;

namespace Nexticz.Module.Mmo.Washing.Application.Printings;

public static class QrGenerator
{
    public static string GetQrCode(string payload)
    {
        using var qrGen = new QRCodeGenerator();
        using var qrData = qrGen.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

        var renderer = new PngByteQRCode(qrData);

        var png = renderer.GetGraphic(20, Color.Black, Color.White, false);
        var base64 = Convert.ToBase64String(png);

        return $"data:image/png;base64,{base64}";
    }
}