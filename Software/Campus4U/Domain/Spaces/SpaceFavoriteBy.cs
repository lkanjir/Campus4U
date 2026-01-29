using Client.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Domain.Spaces
{
    public sealed record SpaceFavoriteBy(
        int ProstorId,
        IReadOnlyList<UserProfile> Korisnici
     );
}
