using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Domain.Spaces
{
    public sealed record UserFavoriteSpaces(
        int KorisnikId,
        IReadOnlyList<Space> Favoriti
    );
}
