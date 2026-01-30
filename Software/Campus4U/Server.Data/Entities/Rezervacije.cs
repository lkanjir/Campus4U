using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Rezervacije
{
    public int Id { get; set; }

    public int ProstorId { get; set; }

    public int KorisnikId { get; set; }

    public DateTime VrijemeOd { get; set; }

    public DateTime VrijemeDo { get; set; }

    public DateTime DatumKreiranja { get; set; }

    public string Status { get; set; } = null!;

    public int BrojOsoba { get; set; }

    public virtual Korisnici Korisnik { get; set; } = null!;

    public virtual Prostori Prostor { get; set; } = null!;
}
