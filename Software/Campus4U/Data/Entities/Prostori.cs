using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;

namespace Client.Data.Entities
{
    [Table("prostori")]
    public partial class Prostori
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("naziv")]
        public string Naziv { get; set; } = null!;

        [Column("kapacitet")]
        public int Kapacitet { get; set; }

        [Column("opremljenost")]
        public string Opremljenost { get; set; } = null!;

        [Column("opis")]
        public string Opis { get; set; } = null!;

        [Column("dom_id")]
        public int DomId { get; set; }

        [Column("tip_prostorije_id")]
        public int TipProstorijeId { get; set; }

        [Column("slika_putanja")]
        public string? SlikaPutanja { get; set; }

        public virtual ICollection<ProstoriFavoriti> ProstoriFavoriti { get; set; } = new List<ProstoriFavoriti>();
    }
}
