using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Tin Posavec

namespace Client.Domain.Menu
{
    public sealed record Meal(
        int JeloId,
        int JelovnikId,
        string Naziv,
        string Kategorija
        );
   
}
