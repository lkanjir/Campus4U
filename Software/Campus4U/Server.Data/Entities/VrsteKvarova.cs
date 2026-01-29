using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class VrsteKvarova
{
    public int VrstaKvaraId { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Kvarovi> Kvarovi { get; set; } = new List<Kvarovi>();
}
