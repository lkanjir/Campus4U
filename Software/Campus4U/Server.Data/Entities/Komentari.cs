using System;
using System.Collections.Generic;

namespace Server.Data.Entities;

public partial class Komentari
{
    public int Id { get; set; }

    public string Sadrzaj { get; set; } = null!;

    public int ObjavaId { get; set; }

    public int AutorId { get; set; }

    public virtual Korisnici Autor { get; set; } = null!;

    public virtual Objave Objava { get; set; } = null!;
}
