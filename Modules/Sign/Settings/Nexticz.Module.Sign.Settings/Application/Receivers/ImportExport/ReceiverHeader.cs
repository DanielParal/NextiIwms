namespace Nexticz.Module.Sign.Settings.Application.Receivers.ImportExport;

internal class ReceiverHeader
{
    public const int Kod = 1;
    public const int Jmeno = 2;
    public const int KodPartnera = 3;
    
    public static string[] ExpectedFileHeader { get; } =
    [
        nameof(Kod),
        nameof(Jmeno),
        nameof(KodPartnera)
    ];
}