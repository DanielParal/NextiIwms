using Microsoft.EntityFrameworkCore;

namespace Nexticz.Module.Vh.Infrastructure.Common.Persistence.VYKHOD;

public partial class VykhodContext : DbContext
{
    public VykhodContext()
    {
    }

    public VykhodContext(DbContextOptions<VykhodContext> options)
        : base(options)
    {
    }

    public virtual DbSet<KDbProc> KDbProcs { get; set; }

    public virtual DbSet<LgsVykonHodnoceni> LgsVykonHodnocenis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Name=ConnectionStrings:KvadosTestConnection");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KDbProc>(entity =>
        {
            entity.HasKey(e => new { e.Idd, e.Idt, e.Idi, e.Idp });

            entity.ToTable("K_DB_PROC");

            entity.Property(e => e.Idd)
                .ValueGeneratedOnAdd()
                .HasColumnName("IDD");
            entity.Property(e => e.Idt).HasColumnName("IDT");
            entity.Property(e => e.Idi).HasColumnName("IDI");
            entity.Property(e => e.Idp).HasColumnName("IDP");
            entity.Property(e => e.Autor)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("AUTOR");
            entity.Property(e => e.AutorI)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("AUTOR_I");
            entity.Property(e => e.Conditions)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("CONDITIONS");
            entity.Property(e => e.GrantRoles)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("GRANT_ROLES");
            entity.Property(e => e.ModifyL)
                .HasDefaultValue((byte)1)
                .HasColumnName("MODIFY_L");
            entity.Property(e => e.ModifydataL).HasColumnName("MODIFYDATA_L");
            entity.Property(e => e.ProcKod)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("PROC_KOD");
            entity.Property(e => e.ProcTyp).HasColumnName("PROC_TYP");
            entity.Property(e => e.Recreate).HasColumnName("RECREATE");
            entity.Property(e => e.SwagKod)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("SWAG_KOD");
            entity.Property(e => e.Ts)
                .HasDefaultValueSql("(CONVERT([datetime],'1900-01-01 00:00:00.000',(121)))")
                .HasColumnType("datetime")
                .HasColumnName("TS");
            entity.Property(e => e.TsI)
                .HasDefaultValueSql("(CONVERT([datetime],'1900-01-01 00:00:00.000',(121)))")
                .HasColumnType("datetime")
                .HasColumnName("TS_I");
            entity.Property(e => e.Valid)
                .HasDefaultValue((byte)1)
                .HasColumnName("VALID");
            entity.Property(e => e.Verzecrp)
                .HasDefaultValueSql("(CONVERT([datetime],'1900-01-01 00:00:00.000',(121)))")
                .HasColumnType("datetime")
                .HasColumnName("VERZECRP");
            entity.Property(e => e.Verzefile)
                .HasDefaultValueSql("(CONVERT([datetime],'1900-01-01 00:00:00.000',(121)))")
                .HasColumnType("datetime")
                .HasColumnName("VERZEFILE");
            entity.Property(e => e.Verzeuprava)
                .HasDefaultValueSql("(CONVERT([datetime],'1900-01-01 00:00:00.000',(121)))")
                .HasColumnType("datetime")
                .HasColumnName("VERZEUPRAVA");
            entity.Property(e => e.Verzevytvoreni)
                .HasDefaultValueSql("(CONVERT([datetime],'1900-01-01 00:00:00.000',(121)))")
                .HasColumnType("datetime")
                .HasColumnName("VERZEVYTVORENI");
        });

        modelBuilder.Entity<LgsVykonHodnoceni>(entity =>
        {
            entity.HasKey(e => new { e.Idd, e.Idt, e.Idi, e.Idp });

            entity.ToTable("LGS_VYKON_HODNOCENI");

            entity.HasIndex(e => e.RowVersion, "IX_ROW_VERSION").IsUnique();

            entity.HasIndex(e => e.ModifyL, "MODIFY_L").HasFilter("([MODIFY_L]=(1))");

            entity.Property(e => e.Idd)
                .ValueGeneratedOnAdd()
                .HasColumnName("IDD");
            entity.Property(e => e.Idt).HasColumnName("IDT");
            entity.Property(e => e.Idi).HasColumnName("IDI");
            entity.Property(e => e.Idp).HasColumnName("IDP");
            entity.Property(e => e.AkceKod)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("AKCE_KOD");
            entity.Property(e => e.Autor)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("AUTOR");
            entity.Property(e => e.AutorI)
                .HasMaxLength(17)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("AUTOR_I");
            entity.Property(e => e.Datum)
                .HasDefaultValueSql("(CONVERT([datetime],'1900-01-01 00:00:00.000',(121)))")
                .HasColumnType("datetime")
                .HasColumnName("DATUM");
            entity.Property(e => e.LicenceKod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("LICENCE_KOD");
            entity.Property(e => e.ModifyL)
                .HasDefaultValue((byte)1)
                .HasColumnName("MODIFY_L");
            entity.Property(e => e.OsobaKod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("OSOBA_KOD");
            entity.Property(e => e.PDoklad)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("P_DOKLAD");
            entity.Property(e => e.PartnerKod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("PARTNER_KOD");
            entity.Property(e => e.PartnerKodExt)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("PARTNER_KOD_EXT");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("ROW_VERSION");
            entity.Property(e => e.SkladKod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("SKLAD_KOD");
            entity.Property(e => e.SkladKodExt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("SKLAD_KOD_EXT");
            entity.Property(e => e.SortKod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("SORT_KOD");
            entity.Property(e => e.SortKodExt)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("SORT_KOD_EXT");
            entity.Property(e => e.Ts)
                .HasDefaultValueSql("(CONVERT([datetime],'1900-01-01 00:00:00.000',(121)))")
                .HasColumnType("datetime")
                .HasColumnName("TS");
            entity.Property(e => e.TsI)
                .HasDefaultValueSql("(CONVERT([datetime],'1900-01-01 00:00:00.000',(121)))")
                .HasColumnType("datetime")
                .HasColumnName("TS_I");
            entity.Property(e => e.UkladatelKod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("UKLADATEL_KOD");
            entity.Property(e => e.UkladatelSkupKod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("UKLADATEL_SKUP_KOD");
            entity.Property(e => e.UmistVychL).HasColumnName("UMIST_VYCH_L");
            entity.Property(e => e.UmisteniKod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("UMISTENI_KOD");
            entity.Property(e => e.Valid)
                .HasDefaultValue((byte)1)
                .HasColumnName("VALID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}