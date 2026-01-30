using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class DnevniJelovnik
{
    public int JelovnikId { get; set; }

    public DateOnly Datum { get; set; }

    public int DanUTjednu { get; set; }

    public DateTime ZadnjeAzurirano { get; set; }

    public virtual ICollection<Jela> Jela { get; set; } = new List<Jela>();
}
