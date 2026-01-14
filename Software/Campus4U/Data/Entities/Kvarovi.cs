using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Client.Data.Entities
{
    [Table("kvarovi")]
    public partial class Kvarovi
    {
        [Key]
        [Column("kvar_id")]
        public int KvarId { get; set; }

        [Column("korisnik_id")]
        public int KorisnikId { get; set; }

        [Column("prostor_id")]
        public int ProstorId { get; set; }

        [Column("vrsta_kvara_id")]
        public int VrstaKvaraId { get; set; }

        [Column("opis")]
        public string Opis { get; set; } = null!;

        [Column("fotografija")]
        public byte[]? Fotografija { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Aktivan";

        [Column("datum_prijave")]
        public DateTime DatumPrijave { get; set; } = DateTime.Now;

        [ForeignKey("KorisnikId")]
        public virtual Korisnici Korisnik { get; set; } = null!;

        [ForeignKey("ProstorId")]
        public virtual Prostori Prostor { get; set; } = null!;

        [ForeignKey("VrstaKvaraId")]
        public virtual VrsteKvarova VrstaKvara { get; set; } = null!;
    }
}
