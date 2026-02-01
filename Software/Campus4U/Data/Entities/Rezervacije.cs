using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client.Data.Entities
{
    [Table("rezervacije")]
    public partial class Rezervacije
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ProstorId")]
        public int ProstorId { get; set; }

        [Column("KorisnikId")]
        public int KorisnikId { get; set; }

        [Column("VrijemeOd")]
        public DateTime VrijemeOd { get; set; }

        [Column("VrijemeDo")]
        public DateTime VrijemeDo { get; set; }

        [Column("DatumKreiranja")]
        public DateTime DatumKreiranja { get; set; }

        [Column("Status")]
        public string Status { get; set; } = null!;
        [Column("BrojOsoba")]
        public int BrojOsoba { get; set; }

        [ForeignKey("ProstorId")]
        public virtual Prostori Prostor { get; set; } = null!;
    }
}
