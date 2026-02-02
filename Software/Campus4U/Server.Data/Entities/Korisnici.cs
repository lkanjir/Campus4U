using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Korisnici
{
    public int Id { get; set; }

    public int UlogaId { get; set; }

    public string Sub { get; set; } = null!;

    public string? Ime { get; set; }

    public string? Prezime { get; set; }

    public string Email { get; set; } = null!;

    public string? BrojSobe { get; set; }

    public string? BrojTelefona { get; set; }

    public string? SlikaProfila { get; set; }

    public string? KorisnickoIme { get; set; }

    public string? SlikaPutanja { get; set; }

    public virtual ICollection<Dogadaji> Dogadaji { get; set; } = new List<Dogadaji>();

    public virtual ICollection<Komentari> Komentari { get; set; } = new List<Komentari>();

    public virtual ICollection<KomentariDogadaja> KomentariDogadaja { get; set; } = new List<KomentariDogadaja>();

    public virtual ICollection<Kvarovi> Kvarovi { get; set; } = new List<Kvarovi>();

    public virtual ICollection<ObavijestiPostavke> ObavijestiPostavke { get; set; } = new List<ObavijestiPostavke>();

    public virtual ICollection<Objave> Objave { get; set; } = new List<Objave>();

    public virtual ICollection<Rezervacije> Rezervacije { get; set; } = new List<Rezervacije>();

    public virtual Uloge Uloga { get; set; } = null!;

    public virtual ICollection<Dogadaji> Dogadaj { get; set; } = new List<Dogadaji>();

    public virtual ICollection<Objave> Objava { get; set; } = new List<Objave>();

    public virtual ICollection<Prostori> Prostor { get; set; } = new List<Prostori>();
}
