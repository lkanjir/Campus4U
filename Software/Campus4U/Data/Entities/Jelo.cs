using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Client.Data.Entities
{
    [Table("jela")]
    public partial class Jelo
    {
        [Key]
        [Column("jelo_id")]
        public int JeloId { get; set; }

        [Column("jelovnik_id")]
        public int JelovnikId { get; set; }

        [Column("naziv")]
        public string Naziv { get; set; } = string.Empty;

        [Column("kategorija")]
        public string Kategorija { get; set; } = string.Empty;

        [ForeignKey("JelovnikId")]
        public virtual DnevniJelovnik DnevniJelovnik { get; set; } = null!;
    }
}
