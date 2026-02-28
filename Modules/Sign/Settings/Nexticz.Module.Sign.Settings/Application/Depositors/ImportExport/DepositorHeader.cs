namespace Nexticz.Module.Sign.Settings.Application.Depositors.ImportExport;

internal class DepositorHeader
{
    public const int Kod = 1;
    public const int Jmeno = 2;
    public const int KodSkupinyUkladatelu = 3;
    public const int KodSablonyDodacihoListu = 4;
    public const int KodSablonyNakladnihoListu = 5;
    
    public static string[] ExpectedFileHeader { get; } =
    [
        nameof(Kod),
        nameof(Jmeno),
        nameof(KodSkupinyUkladatelu),
        nameof(KodSablonyDodacihoListu),
        nameof(KodSablonyNakladnihoListu)
    ];
}