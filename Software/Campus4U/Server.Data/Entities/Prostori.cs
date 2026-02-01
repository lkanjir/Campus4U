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

    public virtual Domovi Dom { get; set; } = null!;

    public virtual ICollection<Kvarovi> Kvarovi { get; set; } = new List<Kvarovi>();

    public virtual ICollection<Rezervacije> Rezervacije { get; set; } = new List<Rezervacije>();

    public virtual TipoviProstora TipProstorije { get; set; } = null!;

    public virtual ICollection<Korisnici> Korisnik { get; set; } = new List<Korisnici>();
}
