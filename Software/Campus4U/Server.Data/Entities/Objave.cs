using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Objave
{
    public int Id { get; set; }

    public string Naslov { get; set; } = null!;

    public string Sadrzaj { get; set; } = null!;

    public DateTime DatumVrijemeObjave { get; set; }

    public DateTime? DatumVrijemeDogadjaja { get; set; }

    public int AutorId { get; set; }

    public virtual Korisnici Autor { get; set; } = null!;

    public virtual ICollection<Komentari> Komentari { get; set; } = new List<Komentari>();

    public virtual ICollection<Korisnici> Korisnik { get; set; } = new List<Korisnici>();
}
