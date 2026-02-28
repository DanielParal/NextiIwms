namespace Nexticz.Module.Cuzk.Application.EconomicSubjects.Models;

public class EconomicSubject
{
    public string? Ico { get; set; }
    public string? ObchodniJmeno { get; set; }
    public Sidlo? Sidlo { get; set; }
    public string? PravniForma { get; set; }
    public string? FinancniUrad { get; set; }
    public DateTime DatumVzniku { get; set; }
    public DateTime DatumZaniku { get; set; }
    public DateTime DatumAktualizace { get; set; }
    public string? Dic { get; set; }
    public string? IcoId { get; set; }
    public AdresaDorucovaci? AdresaDorucovaci { get; set; }
    public SeznamRegistraci? SeznamRegistraci { get; set; }
    public string? PrimarniZdroj { get; set; }
    public List<DalsiUdaje>? DalsiUdaje { get; set; }
    public List<string>? CzNace { get; set; }
    public string? SubRegistrSzr { get; set; }
    public string? DicSkDph { get; set; }
}

public class Sidlo
{
    public string? KodStatu { get; set; }
    public string? NazevStatu { get; set; }
    public int KodKraje { get; set; }
    public string? NazevKraje { get; set; }
    public int KodOkresu { get; set; }
    public string? NazevOkresu { get; set; }
    public int KodObce { get; set; }
    public string? NazevObce { get; set; }
    public int KodSpravnihoObvodu { get; set; }
    public string? NazevSpravnihoObvodu { get; set; }
    public int KodMestskehoObvodu { get; set; }
    public string? NazevMestskehoObvodu { get; set; }
    public int KodMestskeCastiObvodu { get; set; }
    public int KodUlice { get; set; }
    public string? NazevMestskeCastiObvodu { get; set; }
    public string? NazevUlice { get; set; }
    public int CisloDomovni { get; set; }
    public string? DoplnekAdresy { get; set; }
    public int KodCastiObce { get; set; }
    public int CisloOrientacni { get; set; }
    public string? CisloOrientacniPismeno { get; set; }
    public string? NazevCastiObce { get; set; }
    public int KodAdresnihoMista { get; set; }
    public int Psc { get; set; }
    public string? TextovaAdresa { get; set; }
    public string? CisloDoAdresy { get; set; }
    public bool StandardizaceAdresy { get; set; }
    public string? PscTxt { get; set; }
    public int TypCisloDomovni { get; set; }
}

public class AdresaDorucovaci
{
    public string? RadekAdresy1 { get; set; }
    public string? RadekAdresy2 { get; set; }
    public string? RadekAdresy3 { get; set; }
}

public class SeznamRegistraci
{
    public string? StavZdrojeVr { get; set; }
    public string? StavZdrojeRes { get; set; }
    public string? StavZdrojeRzp { get; set; }
    public string? StavZdrojeNrpzs { get; set; }
    public string? StavZdrojeRpsh { get; set; }
    public string? StavZdrojeRcns { get; set; }
    public string? StavZdrojeSzr { get; set; }
    public string? StavZdrojeDph { get; set; }
    public string? StavZdrojeSd { get; set; }
    public string? StavZdrojeIr { get; set; }
    public string? StavZdrojeCeu { get; set; }
    public string? StavZdrojeRs { get; set; }
    public string? StavZdrojeRed { get; set; }
    public string? StavZdrojeMonitor { get; set; }
}

public class ObchodniJmeno
{
    public DateTime PlatnostOd { get; set; }
    public DateTime PlatnostDo { get; set; }
    public string? ObchodniJmenoValue { get; set; }
    public bool PrimarniZaznam { get; set; }
}

public class DalsiSidlo
{
    public Sidlo? Sidlo { get; set; }
    public bool PrimarniZaznam { get; set; }
    public DateTime PlatnostOd { get; set; }
    public DateTime PlatnostDo { get; set; }
}

public class DalsiUdaje
{
    public List<ObchodniJmeno>? ObchodniJmeno { get; set; }
    public List<DalsiSidlo>? Sidlo { get; set; }
    public string? PravniForma { get; set; }
    public string? SpisovaZnacka { get; set; }
    public string? DatovyZdroj { get; set; }
}