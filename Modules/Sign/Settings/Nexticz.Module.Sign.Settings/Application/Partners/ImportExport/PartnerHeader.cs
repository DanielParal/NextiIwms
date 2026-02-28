namespace Nexticz.Module.Sign.Settings.Application.Partners.ImportExport;

internal class PartnerHeader
{
    public const int Kod = 1;
    public const int Jmeno = 2;
    
    public static string[] ExpectedFileHeader { get; } =
    [
        nameof(Kod),
        nameof(Jmeno)
    ];
}