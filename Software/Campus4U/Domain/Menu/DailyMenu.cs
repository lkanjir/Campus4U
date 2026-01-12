using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Tin Posavec

namespace Client.Domain.Menu
{
    public sealed record DailyMenu(
        int JelovnikId,
        DateTime Datum,
        int DanUTjednu,
        DateTime ZadnjeAzurirano,
        List<Meal> Jela
        );
    
}
