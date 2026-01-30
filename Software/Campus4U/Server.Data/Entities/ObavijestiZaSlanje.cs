using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class ObavijestiZaSlanje
{
    public int ObavijestId { get; set; }

    public string Dogadjaj { get; set; } = null!;

    public string Entitet { get; set; } = null!;

    public int EntitetId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime Kreirano { get; set; }

    public int Pokusaji { get; set; }

    public string? ZadnjaGreska { get; set; }
}
