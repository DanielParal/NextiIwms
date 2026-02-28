namespace Nexticz.Module.Sign.Settings.Application.DepositorGroups.ImportExport;

internal class DepositorGroupHeader
{
    public const int Kod = 1;
    public const int Jmeno = 2;
    
    public static string[] ExpectedFileHeader { get; } =
    [
        nameof(Kod),
        nameof(Jmeno)
    ];
}