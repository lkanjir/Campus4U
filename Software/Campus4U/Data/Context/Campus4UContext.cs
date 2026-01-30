using Client.Data.Context.Entities;
using Client.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Identity.Client.Kerberos;

namespace Client.Data.Context;

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

    public virtual DbSet<Uloge> Uloge { get; set; }
    public virtual DbSet<Dogadaji> Dogadaji { get; set; }

    public virtual DbSet<KomentariDogadaja> KomentariDogadaja { get; set; }

    public virtual DbSet<DnevniJelovnik> DnevniJelovnik { get; set; }
    public virtual DbSet<Prostori> Prostori { get; set; }
    public virtual DbSet<DogadajiFavoriti> DogadajiFavoriti { get; set; }
    public virtual DbSet<ProstoriFavoriti> ProstoriFavoriti { get; set; }
    public virtual DbSet<VrsteKvarova> VrsteKvarova { get; set; }
    public virtual DbSet<Kvarovi> Kvarovi { get; set; }

    public virtual DbSet<Jelo> Jelo { get; set; }
    public virtual DbSet<Rezervacije> Rezervacije { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=31.147.206.65;Database=RPP2025_13_DB;User Id=RPP2025_13_User;Password=\"Qic6;,R&oi{drR?r\";TrustServerCertificate=True;");

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
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Ime)
                .HasMaxLength(255)
                .HasColumnName("ime");
            entity.Property(e => e.Prezime)
                .HasMaxLength(255)
                .HasColumnName("prezime");
            entity.Property(e => e.KorisnickoIme)
                .HasMaxLength(50)
                .HasColumnName("korisnicko_ime");
            entity.Property(e => e.Sub)
                .HasMaxLength(255)
                .HasColumnName("sub");
            entity.Property(e => e.UlogaId).HasColumnName("uloga_id");
            entity.Property(e => e.BrojTelefona)
                .HasMaxLength(30)
                .HasColumnName("broj_telefona");
            entity.Property(e => e.SlikaProfila).HasMaxLength(500).HasColumnName("slika_profila");

            entity.HasOne(d => d.Uloga).WithMany(p => p.Korisnici)
                .HasForeignKey(d => d.UlogaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__korisnici__uloga__2C3393D0");
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

        modelBuilder.Entity<Rezervacije>(entity =>
        {
            entity.ToTable("rezervacije", t => t.HasTrigger("trg_rezervacije_obavijesti_za_slanje"));
        });

        modelBuilder.Entity<Dogadaji>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__dogadaji__3213E83FCAC6C6DB");
        });

        modelBuilder.Entity<KomentariDogadaja>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__komentar__3213E83F08A035D5");

            entity.HasOne(d => d.Dogadaj).WithMany(p => p.KomentariDogadaja).HasConstraintName("FK_kd_dogadaji");
        });

        modelBuilder.Entity<Jelo>(entity =>
        {
            entity.HasOne(d => d.DnevniJelovnik)
                .WithMany(p => p.Jela)
                .HasForeignKey(d => d.JelovnikId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Prostori>(entity =>
        {
            entity.ToTable("prostori");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<ProstoriFavoriti>(entity =>
        {
            entity.ToTable("prostori_favoriti");
            entity.HasKey(e => new { e.ProstorId, e.KorisnikId });

            entity.HasOne(d => d.Prostor)
                .WithMany(p => p.ProstoriFavoriti)
                .HasForeignKey(d => d.ProstorId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Fk_pf_prostori");

            entity.HasOne(d => d.Korisnik)
                .WithMany(p => p.ProstoriFavoriti)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Fk_pf_korisnici");
        });

        modelBuilder.Entity<DogadajiFavoriti>(entity =>
        {
            entity.ToTable("Dogadaji_favoriti");
            entity.HasKey(e => new { e.DogadajId, e.KorisnikId })
                .HasName("Pk_dogadaj_favoriti");

            entity.HasOne(d => d.Dogadaj)
                .WithMany(p => p.DogadajiFavoriti)
                .HasForeignKey(d => d.DogadajId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Fk_df_dogadaji");

            entity.HasOne(d => d.Korisnik)
                .WithMany(p => p.DogadajiFavoriti)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("Fk_df_korisnici");
        });

        modelBuilder.Entity<VrsteKvarova>(entity =>
        {
            entity.ToTable("vrste_kvarova");
            entity.HasKey(e => e.VrstaKvaraId);
        });

        modelBuilder.Entity<Kvarovi>(entity =>
        {
            entity.ToTable("kvarovi", t => t.HasTrigger("trg_kvarovi_obavijesti_za_slanje"));
            entity.HasKey(e => e.KvarId);

            entity.HasOne(d => d.Korisnik)
                .WithMany()
                .HasForeignKey(d => d.KorisnikId);

            entity.HasOne(d => d.Prostor)
                .WithMany()
                .HasForeignKey(d => d.ProstorId);

            entity.HasOne(d => d.VrstaKvara)
                .WithMany()
                .HasForeignKey(d => d.VrstaKvaraId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
