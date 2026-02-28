namespace Nexticz.Module.Cuzk.Application.Municipalities.ImportExport;

internal class MunicipalityHeader
{
    public static string[] ExpectedFileHeader { get; } =
    [
        "Kód",
        "Název Obce",
        "Status obce",
        "Kód POU",
        "Název POU",
        "Kód ORP",
        "Název ORP",
        "Kód Okresu",
        "Název Okresu",
        "Kód Kraje (VÚSC)",
        "Název Kraje (VÚSC)"
    ];
}