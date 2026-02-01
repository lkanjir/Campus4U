using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class ObavijestiPostavke
{
    public int KorisnikId { get; set; }

    public string Tip { get; set; } = null!;

    public bool Omoguceno { get; set; }

    public DateTime Azurirano { get; set; }

    public virtual Korisnici Korisnik { get; set; } = null!;
}
