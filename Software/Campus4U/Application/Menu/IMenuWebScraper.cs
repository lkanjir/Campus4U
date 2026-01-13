using Client.Domain.Menu;

//Tin Posavec

namespace Client.Application.Menu
{
    public interface IMenuWebScraper
    {
        Task<IEnumerable<DailyMenu>> DohvatiJelovnikSWeba();
    }
}
