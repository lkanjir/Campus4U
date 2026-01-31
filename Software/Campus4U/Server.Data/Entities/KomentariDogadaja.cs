using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class KomentariDogadaja
{
    public int Id { get; set; }

    public DateTime Datum { get; set; }

    public int Ocjena { get; set; }

    public string? Komentar { get; set; }

    public int DogadajId { get; set; }

    public int KorisnikId { get; set; }

    public virtual Dogadaji Dogadaj { get; set; } = null!;

    public virtual Korisnici Korisnik { get; set; } = null!;
}
