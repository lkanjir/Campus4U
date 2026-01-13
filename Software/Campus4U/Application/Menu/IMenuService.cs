using Client.Domain.Menu;

//Tin Posavec

namespace Client.Application.Menu
{
    public interface IMenuService
    {
        Task<IEnumerable<DailyMenu>> DohvatiJelovnikZaTjedan();
        Task<IEnumerable<DailyMenu>> OsvjeziJelovnikSWeba();
        Task<DailyMenu?> DohvatiJelovnikZaDatum(DateTime datum);

       
    }
}
