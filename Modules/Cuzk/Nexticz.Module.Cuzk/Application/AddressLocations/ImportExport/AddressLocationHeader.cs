namespace Nexticz.Module.Cuzk.Application.AddressLocations.ImportExport;

internal class AddressLocationHeader
{
    
    public static string[] ExpectedFileHeader { get; } =
    [
        "Kód ADM",
        "Kód obce",
        "Název obce",
        "Kód MOMC",
        "Název MOMC",
        "Kód obvodu Prahy",
        "Název obvodu Prahy",
        "Kód části obce",
        "Název části obce",
        "Kód ulice",
        "Název ulice",
        "Typ SO",
        "Číslo domovní",
        "Číslo orientační",
        "Znak čísla orientačního",
        "PSČ",
        "Souřadnice Y",
        "Souřadnice X",
        "Platí Od"
    ];
}