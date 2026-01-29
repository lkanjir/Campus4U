using System.ComponentModel.DataAnnotations.Schema;

namespace Client.Data.Entities
{
    [Table("prostori_favoriti")]
    public partial class ProstoriFavoriti
    {
        [Column("prostor_id")]
        public int ProstorId { get; set; }

        [Column("korisnik_id")]
        public int KorisnikId { get; set; }

        public virtual Prostori Prostor { get; set; } = null!;
        public virtual Korisnici Korisnik { get; set; } = null!;
    }
}
