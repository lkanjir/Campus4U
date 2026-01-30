using System.ComponentModel.DataAnnotations.Schema;
using Client.Data.Context.Entities;

namespace Client.Data.Entities
{
    [Table("Dogadaji_favoriti")]
    public partial class DogadajiFavoriti
    {
        [Column("dogadaj_id")]
        public int DogadajId { get; set; }

        [Column("korisnik_id")]
        public int KorisnikId { get; set; }

        public virtual Dogadaji Dogadaj { get; set; } = null!;
        public virtual Korisnici Korisnik { get; set; } = null!;
    }
}
