using System;
using System.Collections.Generic;

namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.VYKHOD;

public partial class KDbProc
{
    public string ProcKod { get; set; } = null!;

    public byte ProcTyp { get; set; }

    public DateTime Verzecrp { get; set; }

    public DateTime Verzefile { get; set; }

    public DateTime Verzevytvoreni { get; set; }

    public DateTime Verzeuprava { get; set; }

    public string SwagKod { get; set; } = null!;

    public int Idd { get; set; }

    public short Idt { get; set; }

    public short Idi { get; set; }

    public int Idp { get; set; }

    public byte Valid { get; set; }

    public string Autor { get; set; } = null!;

    public DateTime Ts { get; set; }

    public string AutorI { get; set; } = null!;

    public DateTime TsI { get; set; }

    public byte ModifyL { get; set; }

    public byte ModifydataL { get; set; }

    public string GrantRoles { get; set; } = null!;

    public byte Recreate { get; set; }

    public string Conditions { get; set; } = null!;
}
