using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Uloge
{
    public int Id { get; set; }

    public string NazivUloge { get; set; } = null!;

    public virtual ICollection<Korisnici> Korisnici { get; set; } = new List<Korisnici>();
}
