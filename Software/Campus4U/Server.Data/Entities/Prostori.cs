using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Prostori
{
    public int Id { get; set; }

    public string Naziv { get; set; } = null!;

    public int Kapacitet { get; set; }

    public string Opremljenost { get; set; } = null!;

    public string Opis { get; set; } = null!;

    public int DomId { get; set; }

    public int TipProstorijeId { get; set; }

    public string? SlikaPutanja { get; set; }

    public virtual ICollection<Kvarovi> Kvarovi { get; set; } = new List<Kvarovi>();
}
