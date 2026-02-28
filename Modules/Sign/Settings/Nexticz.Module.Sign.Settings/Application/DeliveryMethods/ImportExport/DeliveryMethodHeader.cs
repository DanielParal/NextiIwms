namespace Nexticz.Module.Sign.Settings.Application.DeliveryMethods.ImportExport;

internal class DeliveryMethodHeader
{
    public const int Kod = 1;
    public const int Jmeno = 2;
    public const int PocetKopiiProNakladniList = 3;
    public const int PocetKopiiProDodaciList = 4;
    
    public static string[] ExpectedFileHeader { get; } =
    [
        nameof(Kod),
        nameof(Jmeno),
        nameof(PocetKopiiProNakladniList),
        nameof(PocetKopiiProDodaciList)
    ];
}