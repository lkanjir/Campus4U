using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Kvarovi
{
    public int KvarId { get; set; }

    public int KorisnikId { get; set; }

    public int ProstorId { get; set; }

    public int VrstaKvaraId { get; set; }

    public string Opis { get; set; } = null!;

    public byte[]? Fotografija { get; set; }

    public string Status { get; set; } = null!;

    public DateTime DatumPrijave { get; set; }

    public string? SlikaPutanja { get; set; }

    public virtual Korisnici Korisnik { get; set; } = null!;

    public virtual Prostori Prostor { get; set; } = null!;

    public virtual VrsteKvarova VrstaKvara { get; set; } = null!;
}
