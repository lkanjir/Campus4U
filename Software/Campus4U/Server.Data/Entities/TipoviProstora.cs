using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class TipoviProstora
{
    public int Id { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Prostori> Prostori { get; set; } = new List<Prostori>();
}
