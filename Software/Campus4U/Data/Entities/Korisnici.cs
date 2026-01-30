using System;
using System.Collections.Generic;

namespace Client.Data.Entities;

public partial class Korisnici
{
    public int Id { get; set; }

    public int UlogaId { get; set; }

    public string Sub { get; set; } = null!;

    public string? Ime { get; set; }

    public string? Prezime { get; set; }

    public string? KorisnickoIme { get; set; }

    public string Email { get; set; } = null!;

    public string? BrojSobe { get; set; }

    public string? BrojTelefona { get; set; }

    public string? SlikaProfila { get; set; }

    public virtual Uloge Uloga { get; set; } = null!;

    public virtual ICollection<DogadajiFavoriti> DogadajiFavoriti { get; set; } = new List<DogadajiFavoriti>();

    public virtual ICollection<ProstoriFavoriti> ProstoriFavoriti { get; set; } = new List<ProstoriFavoriti>();
}
