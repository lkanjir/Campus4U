using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data.Entities
{
    [Table("dnevni_jelovnik")]
    public partial class DnevniJelovnik
    {
        [Key]
        [Column("jelovnik_id")]
        public int JelovnikId { get; set; }

        [Column("datum")]
        public DateTime Datum { get; set; }

        [Column("dan_u_tjednu")]
        public int DanUTjednu { get; set; }

        [Column("zadnje_azurirano")]
        public DateTime ZadnjeAzurirano { get; set; }

        public virtual ICollection<Jelo> Jela { get; set; } = new List<Jelo>();
    }
}
