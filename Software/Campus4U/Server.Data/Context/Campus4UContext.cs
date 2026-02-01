using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Server.Data.Entities;

namespace Server.Data.Context;

public partial class Campus4UContext : DbContext
{
    public Campus4UContext(DbContextOptions<Campus4UContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DnevniJelovnik> DnevniJelovnik { get; set; }

    public virtual DbSet<Dogadaji> Dogadaji { get; set; }

    public virtual DbSet<Domovi> Domovi { get; set; }

    public virtual DbSet<Jela> Jela { get; set; }

    public virtual DbSet<Komentari> Komentari { get; set; }

    public virtual DbSet<KomentariDogadaja> KomentariDogadaja { get; set; }

    public virtual DbSet<Korisnici> Korisnici { get; set; }

    public virtual DbSet<Kvarovi> Kvarovi { get; set; }

    public virtual DbSet<ObavijestiPostavke> ObavijestiPostavke { get; set; }

    public virtual DbSet<ObavijestiZaSlanje> ObavijestiZaSlanje { get; set; }

    public virtual DbSet<Objave> Objave { get; set; }

    public virtual DbSet<Prostori> Prostori { get; set; }

    public virtual DbSet<Rezervacije> Rezervacije { get; set; }

    public virtual DbSet<TipoviProstora> TipoviProstora { get; set; }

    public virtual DbSet<Uloge> Uloge { get; set; }

    public virtual DbSet<VrsteKvarova> VrsteKvarova { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DnevniJelovnik>(entity =>
        {
            entity.HasKey(e => e.JelovnikId).HasName("PK__DnevniJe__9868A9F769FB27E1");

            entity.ToTable("dnevni_jelovnik");

            entity.HasIndex(e => e.Datum, "UQ_DnevniJelovnik_Datum").IsUnique();

            entity.Property(e => e.JelovnikId).HasColumnName("jelovnik_id");
            entity.Property(e => e.DanUTjednu).HasColumnName("dan_u_tjednu");
            entity.Property(e => e.Datum).HasColumnName("datum");
            entity.Property(e => e.ZadnjeAzurirano)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("zadnje_azurirano");
        });

        modelBuilder.Entity<Dogadaji>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__dogadaji__3213E83FCAC6C6DB");

            entity.ToTable("dogadaji");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutorId).HasColumnName("autor_id");
            entity.Property(e => e.Naslov)
                .HasMaxLength(255)
                .HasColumnName("naslov");
            entity.Property(e => e.Opis)
                .HasMaxLength(1000)
                .HasColumnName("opis");
            entity.Property(e => e.Slika)
                .HasMaxLength(2000)
                .HasColumnName("slika");
            entity.Property(e => e.SlikaPutanja)
                .HasMaxLength(500)
                .HasColumnName("slika_putanja");
            entity.Property(e => e.VrijemeDogadaja).HasColumnName("vrijeme_dogadaja");
            entity.Property(e => e.VrijemeObjave).HasColumnName("vrijeme_objave");

            entity.HasOne(d => d.Autor).WithMany(p => p.Dogadaji)
                .HasForeignKey(d => d.AutorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_dogadaji_korisnici_autor");

            entity.HasMany(d => d.Korisnik).WithMany(p => p.Dogadaj)
                .UsingEntity<Dictionary<string, object>>(
                    "DogadajiFavoriti",
                    r => r.HasOne<Korisnici>().WithMany()
                        .HasForeignKey("KorisnikId")
                        .HasConstraintName("FK_df_korisnici"),
                    l => l.HasOne<Dogadaji>().WithMany()
                        .HasForeignKey("DogadajId")
                        .HasConstraintName("FK_df_dogadaji"),
                    j =>
                    {
                        j.HasKey("DogadajId", "KorisnikId").HasName("PK_dogadaj_favoriti");
                        j.ToTable("dogadaji_favoriti");
                        j.IndexerProperty<int>("DogadajId").HasColumnName("dogadaj_id");
                        j.IndexerProperty<int>("KorisnikId").HasColumnName("korisnik_id");
                    });
        });

        modelBuilder.Entity<Domovi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__domovi__3213E83F959815D8");

            entity.ToTable("domovi");

            entity.HasIndex(e => e.NazivDoma, "UQ__domovi__364940CE26A79BFE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NazivDoma)
                .HasMaxLength(50)
                .HasColumnName("naziv_doma");
        });

        modelBuilder.Entity<Jela>(entity =>
        {
            entity.HasKey(e => e.JeloId).HasName("PK__Jela__A93B7080FCC3F758");

            entity.ToTable("jela");

            entity.Property(e => e.JeloId).HasColumnName("jelo_id");
            entity.Property(e => e.JelovnikId).HasColumnName("jelovnik_id");
            entity.Property(e => e.Kategorija)
                .HasMaxLength(50)
                .HasColumnName("kategorija");
            entity.Property(e => e.Naziv)
                .HasMaxLength(255)
                .HasColumnName("naziv");

            entity.HasOne(d => d.Jelovnik).WithMany(p => p.Jela)
                .HasForeignKey(d => d.JelovnikId)
                .HasConstraintName("FK_Jela_DnevniJelovnik");
        });

        modelBuilder.Entity<Komentari>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__komentar__3213E83F476DEF1D");

            entity.ToTable("komentari");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutorId).HasColumnName("autor_id");
            entity.Property(e => e.ObjavaId).HasColumnName("objava_id");
            entity.Property(e => e.Sadrzaj).HasColumnName("sadrzaj");

            entity.HasOne(d => d.Autor).WithMany(p => p.Komentari)
                .HasForeignKey(d => d.AutorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__komentari__autor__37A5467C");

            entity.HasOne(d => d.Objava).WithMany(p => p.Komentari)
                .HasForeignKey(d => d.ObjavaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__komentari__objav__36B12243");
        });

        modelBuilder.Entity<KomentariDogadaja>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__komentar__3213E83F08A035D5");

            entity.ToTable("komentari_dogadaja");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datum).HasColumnName("datum");
            entity.Property(e => e.DogadajId).HasColumnName("dogadaj_id");
            entity.Property(e => e.Komentar)
                .HasMaxLength(1000)
                .HasColumnName("komentar");
            entity.Property(e => e.KorisnikId).HasColumnName("korisnik_id");
            entity.Property(e => e.Ocjena).HasColumnName("ocjena");

            entity.HasOne(d => d.Dogadaj).WithMany(p => p.KomentariDogadaja)
                .HasForeignKey(d => d.DogadajId)
                .HasConstraintName("FK_kd_dogadaji");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.KomentariDogadaja)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK_kd_korisnici");
        });

        modelBuilder.Entity<Korisnici>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__korisnic__3213E83F54EBF408");

            entity.ToTable("korisnici");

            entity.HasIndex(e => e.Email, "UQ__korisnic__AB6E61642276BC97").IsUnique();

            entity.HasIndex(e => e.Sub, "UQ__korisnic__DDDF3AD8466C9646").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrojSobe)
                .HasMaxLength(50)
                .HasColumnName("broj_sobe");
            entity.Property(e => e.BrojTelefona)
                .HasMaxLength(30)
                .HasColumnName("broj_telefona");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Ime)
                .HasMaxLength(255)
                .HasColumnName("ime");
            entity.Property(e => e.KorisnickoIme)
                .HasMaxLength(50)
                .HasColumnName("korisnicko_ime");
            entity.Property(e => e.Prezime)
                .HasMaxLength(255)
                .HasColumnName("prezime");
            entity.Property(e => e.SlikaProfila)
                .HasMaxLength(500)
                .HasColumnName("slika_profila");
            entity.Property(e => e.SlikaPutanja)
                .HasMaxLength(500)
                .HasColumnName("slika_putanja");
            entity.Property(e => e.Sub)
                .HasMaxLength(255)
                .HasColumnName("sub");
            entity.Property(e => e.UlogaId).HasColumnName("uloga_id");

            entity.HasOne(d => d.Uloga).WithMany(p => p.Korisnici)
                .HasForeignKey(d => d.UlogaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__korisnici__uloga__2C3393D0");
        });

        modelBuilder.Entity<Kvarovi>(entity =>
        {
            entity.HasKey(e => e.KvarId).HasName("PK__kvarovi__1398CFD5BCED9F4F");

            entity.ToTable("kvarovi", tb => tb.HasTrigger("trg_kvarovi_obavijesti_za_slanje"));

            entity.Property(e => e.KvarId).HasColumnName("kvar_id");
            entity.Property(e => e.DatumPrijave)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("datum_prijave");
            entity.Property(e => e.Fotografija).HasColumnName("fotografija");
            entity.Property(e => e.KorisnikId).HasColumnName("korisnik_id");
            entity.Property(e => e.Opis).HasColumnName("opis");
            entity.Property(e => e.ProstorId).HasColumnName("prostor_id");
            entity.Property(e => e.SlikaPutanja)
                .HasMaxLength(500)
                .HasColumnName("slika_putanja");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Aktivan")
                .HasColumnName("status");
            entity.Property(e => e.VrstaKvaraId).HasColumnName("vrsta_kvara_id");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Kvarovi)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK_Kvarovi_Korisnici");

            entity.HasOne(d => d.Prostor).WithMany(p => p.Kvarovi)
                .HasForeignKey(d => d.ProstorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Kvarovi_Prostori");

            entity.HasOne(d => d.VrstaKvara).WithMany(p => p.Kvarovi)
                .HasForeignKey(d => d.VrstaKvaraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Kvarovi_VrsteKvarova");
        });

        modelBuilder.Entity<ObavijestiPostavke>(entity =>
        {
            entity.HasKey(e => new { e.KorisnikId, e.Tip });

            entity.ToTable("obavijesti_postavke");

            entity.HasIndex(e => new { e.Tip, e.Omoguceno }, "IX_obavijesti_postavke_tip_omoguceno");

            entity.Property(e => e.KorisnikId).HasColumnName("korisnik_id");
            entity.Property(e => e.Tip)
                .HasMaxLength(50)
                .HasColumnName("tip");
            entity.Property(e => e.Azurirano)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("azurirano");
            entity.Property(e => e.Omoguceno)
                .HasDefaultValue(true)
                .HasColumnName("omoguceno");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.ObavijestiPostavke)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK_obavijesti_postavke_korisnici");
        });

        modelBuilder.Entity<ObavijestiZaSlanje>(entity =>
        {
            entity.HasKey(e => e.ObavijestId).HasName("PK__obavijes__C5D55EAA4BC6A9AC");

            entity.ToTable("obavijesti_za_slanje");

            entity.HasIndex(e => new { e.Status, e.Kreirano }, "IDX_obavijesti_za_slanje_status_kreirano");

            entity.Property(e => e.ObavijestId).HasColumnName("obavijest_id");
            entity.Property(e => e.Dogadjaj)
                .HasMaxLength(100)
                .HasColumnName("dogadjaj");
            entity.Property(e => e.Entitet)
                .HasMaxLength(50)
                .HasColumnName("entitet");
            entity.Property(e => e.EntitetId).HasColumnName("entitet_id");
            entity.Property(e => e.Kreirano)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("kreirano");
            entity.Property(e => e.Pokusaji).HasColumnName("pokusaji");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ceka")
                .HasColumnName("status");
            entity.Property(e => e.ZadnjaGreska)
                .HasMaxLength(2000)
                .HasColumnName("zadnja_greska");
        });

        modelBuilder.Entity<Objave>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__objave__3213E83F7BB0EC33");

            entity.ToTable("objave");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutorId).HasColumnName("autor_id");
            entity.Property(e => e.DatumVrijemeDogadjaja)
                .HasColumnType("datetime")
                .HasColumnName("datum_vrijeme_dogadjaja");
            entity.Property(e => e.DatumVrijemeObjave)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("datum_vrijeme_objave");
            entity.Property(e => e.Naslov)
                .HasMaxLength(255)
                .HasColumnName("naslov");
            entity.Property(e => e.Sadrzaj).HasColumnName("sadrzaj");

            entity.HasOne(d => d.Autor).WithMany(p => p.Objave)
                .HasForeignKey(d => d.AutorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__objave__autor_id__300424B4");

            entity.HasMany(d => d.Korisnik).WithMany(p => p.Objava)
                .UsingEntity<Dictionary<string, object>>(
                    "Interesi",
                    r => r.HasOne<Korisnici>().WithMany()
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__interes__korisni__33D4B598"),
                    l => l.HasOne<Objave>().WithMany()
                        .HasForeignKey("ObjavaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__interes__objava___32E0915F"),
                    j =>
                    {
                        j.HasKey("ObjavaId", "KorisnikId").HasName("PK__interes__759A98B1B12CD6D9");
                        j.ToTable("interesi");
                        j.IndexerProperty<int>("ObjavaId").HasColumnName("objava_id");
                        j.IndexerProperty<int>("KorisnikId").HasColumnName("korisnik_id");
                    });
        });

        modelBuilder.Entity<Prostori>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__prostori__3213E83FFAE8E726");

            entity.ToTable("prostori");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DomId).HasColumnName("dom_id");
            entity.Property(e => e.Kapacitet).HasColumnName("kapacitet");
            entity.Property(e => e.Naziv)
                .HasMaxLength(100)
                .HasColumnName("naziv");
            entity.Property(e => e.Opis)
                .HasMaxLength(500)
                .HasColumnName("opis");
            entity.Property(e => e.Opremljenost)
                .HasMaxLength(255)
                .HasColumnName("opremljenost");
            entity.Property(e => e.SlikaPutanja)
                .HasMaxLength(255)
                .HasColumnName("slika_putanja");
            entity.Property(e => e.TipProstorijeId).HasColumnName("tip_prostorije_id");

            entity.HasOne(d => d.Dom).WithMany(p => p.Prostori)
                .HasForeignKey(d => d.DomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_prostori_domovi");

            entity.HasOne(d => d.TipProstorije).WithMany(p => p.Prostori)
                .HasForeignKey(d => d.TipProstorijeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_prostori_tipovi");

            entity.HasMany(d => d.Korisnik).WithMany(p => p.Prostor)
                .UsingEntity<Dictionary<string, object>>(
                    "ProstoriFavoriti",
                    r => r.HasOne<Korisnici>().WithMany()
                        .HasForeignKey("KorisnikId")
                        .HasConstraintName("FK_pf_korisnici"),
                    l => l.HasOne<Prostori>().WithMany()
                        .HasForeignKey("ProstorId")
                        .HasConstraintName("FK_pf_prostori"),
                    j =>
                    {
                        j.HasKey("ProstorId", "KorisnikId");
                        j.ToTable("prostori_favoriti");
                        j.IndexerProperty<int>("ProstorId").HasColumnName("prostor_id");
                        j.IndexerProperty<int>("KorisnikId").HasColumnName("korisnik_id");
                    });
        });

        modelBuilder.Entity<Rezervacije>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__rezervac__3214EC073B9B72D5");

            entity.ToTable("rezervacije", tb => tb.HasTrigger("trg_rezervacije_obavijesti_za_slanje"));

            entity.Property(e => e.DatumKreiranja).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Aktivna");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Rezervacije)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK_rezervacije_korisnici");

            entity.HasOne(d => d.Prostor).WithMany(p => p.Rezervacije)
                .HasForeignKey(d => d.ProstorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_rezervacije_prostori");
        });

        modelBuilder.Entity<TipoviProstora>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tipovi_p__3213E83F4069A873");

            entity.ToTable("tipovi_prostora");

            entity.HasIndex(e => e.Naziv, "UQ__tipovi_p__F07241F3375A7E55").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Naziv)
                .HasMaxLength(50)
                .HasColumnName("naziv");
        });

        modelBuilder.Entity<Uloge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("uloge_pk");

            entity.ToTable("uloge");

            entity.HasIndex(e => e.NazivUloge, "UQ__uloge__690D1BE5F0F82B00").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NazivUloge)
                .HasMaxLength(50)
                .HasColumnName("naziv_uloge");
        });

        modelBuilder.Entity<VrsteKvarova>(entity =>
        {
            entity.HasKey(e => e.VrstaKvaraId).HasName("PK__vrste_kv__E36A21033A2C26EE");

            entity.ToTable("vrste_kvarova");

            entity.Property(e => e.VrstaKvaraId).HasColumnName("vrsta_kvara_id");
            entity.Property(e => e.Naziv)
                .HasMaxLength(100)
                .HasColumnName("naziv");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
