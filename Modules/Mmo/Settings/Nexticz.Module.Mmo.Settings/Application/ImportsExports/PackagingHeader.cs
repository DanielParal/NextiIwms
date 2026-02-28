namespace Nexticz.Module.Mmo.Settings.Application.ImportsExports;

internal static class PackagingHeader
{
    public const int ZakaznickeCisloObalu = 1;
    public const int KodUkladatele = 2;
    public const int KodTypuObalu = 3;
    public const int KodObehovostiObalu = 4;
    public const int NutnoPrat = 5;
    public const int NazevObalu = 6;
    public const int Hloubka = 7;
    public const int Sirka = 8;
    public const int Vyska = 9;
    public const int Hmotnost = 10;
    public const int RychlostiMycek = 11;
    
    public static string[] ExpectedFileHeader { get; } =
    [
        nameof(ZakaznickeCisloObalu),
        nameof(KodUkladatele),
        nameof(KodTypuObalu),
        nameof(KodObehovostiObalu),
        nameof(NutnoPrat),
        nameof(NazevObalu),
        nameof(Hloubka), 
        nameof(Sirka),
        nameof(Vyska),
        nameof(Hmotnost),
        nameof(RychlostiMycek)
    ];
}