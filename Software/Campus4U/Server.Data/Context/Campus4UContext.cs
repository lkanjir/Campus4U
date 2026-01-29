using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Server.Data.Entities;

namespace Server.Data.Context;

public partial class Campus4UContext : DbContext
{
    public Campus4UContext()
    {
    }

    public Campus4UContext(DbContextOptions<Campus4UContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Korisnici> Korisnici { get; set; }

    public virtual DbSet<Kvarovi> Kvarovi { get; set; }

    public virtual DbSet<ObavijestiZaSlanje> ObavijestiZaSlanje { get; set; }

    public virtual DbSet<Prostori> Prostori { get; set; }

    public virtual DbSet<VrsteKvarova> VrsteKvarova { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            entity.Property(e => e.Sub)
                .HasMaxLength(255)
                .HasColumnName("sub");
            entity.Property(e => e.UlogaId).HasColumnName("uloga_id");
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
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Aktivan")
                .HasColumnName("status");
            entity.Property(e => e.VrstaKvaraId).HasColumnName("vrsta_kvara_id");

            entity.HasOne(d => d.Korisnik).WithMany(p => p.Kvarovi)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
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
