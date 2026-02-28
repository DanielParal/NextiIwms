namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports;

internal class KitHeader
{
    public const int CisloKitu = 1;
    public const int KodUkladatele = 2;
    public const int KodTypuKitu = 3;
    public const int KodDefiniceSapuKitu = 4;
    public const int KodVyroby = 5;
    public const int KodUrcujicihoBaleni = 6;
    public const int KodBaleni = 7;
    public const int PocetBaleniVKitu = 8;
    public const int Poznamka = 9;
    public const int CasChladnuti = 10;
    public const int ObsahujeBaliciPredpis = 11;
    
    public static string[] ExpectedFileHeader { get; } =
    [
        nameof(CisloKitu),
        nameof(KodUkladatele),
        nameof(KodTypuKitu),
        nameof(KodDefiniceSapuKitu),
        nameof(KodVyroby),
        nameof(KodUrcujicihoBaleni),
        nameof(KodBaleni),
        nameof(PocetBaleniVKitu), 
        nameof(Poznamka),
        nameof(CasChladnuti),
        nameof(ObsahujeBaliciPredpis)
    ];
}