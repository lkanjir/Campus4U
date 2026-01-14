using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Client.Data.Entities
{
    [Table("vrste_kvarova")]
    public partial class VrsteKvarova
    {
        [Key]
        [Column("vrsta_kvara_id")]
        public int VrstaKvaraId { get; set; }

        [Column("naziv")]
        public string Naziv { get; set; } = null!;
    }
}
