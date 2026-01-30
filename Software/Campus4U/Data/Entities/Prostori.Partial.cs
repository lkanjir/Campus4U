using System.Collections.Generic;

namespace Client.Data.Entities
{
    public partial class Prostori
    {
        public virtual ICollection<ProstoriFavoriti> ProstoriFavoriti { get; set; } =
            new List<ProstoriFavoriti>();
    }
}
