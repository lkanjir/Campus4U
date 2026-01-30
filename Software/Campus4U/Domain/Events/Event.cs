using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Domain.Events
{
    public sealed record Event(
        int Id,
        string Naslov,
        string Opis,
        DateTime VrijemeObjave,
        DateTime VrijemeDogadaja,
        byte[] Slika
        );
}
