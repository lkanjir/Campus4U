using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Domain.Menu;

//Tin Posavec

namespace Client.Application.Menu
{
    public interface IMenuRepository
    {
        Task<IEnumerable<DailyMenu>> DohvatiSveJelovnike();
        Task<DailyMenu?> DohvatiJelovnikZaDatum(DateTime date);

        Task<IEnumerable<DailyMenu>> DohvatiTjedniJelovnik(DateTime pocetakTjedna);

        bool SpremiJelovnik(DailyMenu jelovnik);

        bool SpremiViseJelovnika(IEnumerable<DailyMenu> jelovnici);

        bool ObrisiStareJelovnike(DateTime datum);

        Task<DateTime?> DohvatiDatumZadnjegAzuriranja();

        bool IsJelovnikAzuriran(DateTime datum);
    }
}
