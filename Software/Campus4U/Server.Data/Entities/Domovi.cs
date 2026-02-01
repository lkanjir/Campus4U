using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Domovi
{
    public int Id { get; set; }

    public string NazivDoma { get; set; } = null!;

    public virtual ICollection<Prostori> Prostori { get; set; } = new List<Prostori>();
}
