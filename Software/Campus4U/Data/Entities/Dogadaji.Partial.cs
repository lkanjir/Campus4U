using System.Collections.Generic;
using Client.Data.Entities;

namespace Client.Data.Context.Entities
{
    public partial class Dogadaji
    {
        public virtual ICollection<DogadajiFavoriti> DogadajiFavoriti { get; set; } =
            new List<DogadajiFavoriti>();
    }
}
