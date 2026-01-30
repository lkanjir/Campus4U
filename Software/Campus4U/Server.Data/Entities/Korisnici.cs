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

    public virtual ICollection<Kvarovi> Kvarovi { get; set; } = new List<Kvarovi>();

    public virtual ICollection<Rezervacije> Rezervacije { get; set; } = new List<Rezervacije>();
}
