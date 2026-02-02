using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Dogadaji
{
    public int Id { get; set; }

    public string Naslov { get; set; } = null!;

    public string Opis { get; set; } = null!;

    public DateTime VrijemeObjave { get; set; }

    public DateTime VrijemeDogadaja { get; set; }

    public byte[] Slika { get; set; } = null!;

    public string? SlikaPutanja { get; set; }

    public int? AutorId { get; set; }

    public virtual Korisnici? Autor { get; set; }

    public virtual ICollection<KomentariDogadaja> KomentariDogadaja { get; set; } = new List<KomentariDogadaja>();

    public virtual ICollection<Korisnici> Korisnik { get; set; } = new List<Korisnici>();
}
