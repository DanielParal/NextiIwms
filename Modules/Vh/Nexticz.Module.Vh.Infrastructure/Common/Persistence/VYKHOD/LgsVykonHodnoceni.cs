using System;
using System.Collections.Generic;

namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.VYKHOD;

public partial class LgsVykonHodnoceni
{
    public string LicenceKod { get; set; } = null!;

    public string AkceKod { get; set; } = null!;

    public DateTime Datum { get; set; }

    public string OsobaKod { get; set; } = null!;

    public string UkladatelKod { get; set; } = null!;

    public string UkladatelSkupKod { get; set; } = null!;

    public string SkladKod { get; set; } = null!;

    public string SkladKodExt { get; set; } = null!;

    public string SortKod { get; set; } = null!;

    public string SortKodExt { get; set; } = null!;

    public string PartnerKod { get; set; } = null!;

    public string PartnerKodExt { get; set; } = null!;

    public string UmisteniKod { get; set; } = null!;

    public byte UmistVychL { get; set; }

    public byte Valid { get; set; }

    public int Idd { get; set; }

    public short Idt { get; set; }

    public short Idi { get; set; }

    public int Idp { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public string Autor { get; set; } = null!;

    public DateTime Ts { get; set; }

    public string AutorI { get; set; } = null!;

    public DateTime TsI { get; set; }

    public byte ModifyL { get; set; }

    public string PDoklad { get; set; } = null!;
}
