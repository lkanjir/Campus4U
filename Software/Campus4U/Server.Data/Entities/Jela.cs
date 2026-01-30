using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Jela
{
    public int JeloId { get; set; }

    public int JelovnikId { get; set; }

    public string Naziv { get; set; } = null!;

    public string Kategorija { get; set; } = null!;

    public virtual DnevniJelovnik Jelovnik { get; set; } = null!;
}
