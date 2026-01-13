using Client.Domain.Menu;

//Tin Posavec

namespace Client.Application.Menu
{
    public sealed class MenuService : IMenuService
    {
        private readonly IMenuRepository _repository;
        private readonly IMenuWebScraper _webScraper;

        public MenuService(IMenuRepository repository, IMenuWebScraper webScraper)
        {
            _repository = repository;
            _webScraper = webScraper;
        }

        public async Task<IEnumerable<DailyMenu>> DohvatiJelovnikZaTjedan()
        {
            var pocetakTjedna = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

            if (_repository.IsJelovnikAzuriran(DateTime.Today))
            {
                return await _repository.DohvatiTjedniJelovnik(pocetakTjedna);
            }

            return await OsvjeziJelovnikSWeba();
        }

        public async Task<IEnumerable<DailyMenu>> OsvjeziJelovnikSWeba()
        {
            var jelovnici = await _webScraper.DohvatiJelovnikSWeba();

            _repository.ObrisiStareJelovnike(DateTime.Today.AddDays(-30));
            _repository.SpremiViseJelovnika(jelovnici);

            return jelovnici;
        }

        public async Task<DailyMenu?> DohvatiJelovnikZaDatum(DateTime datum)
        {
            return await _repository.DohvatiJelovnikZaDatum(datum);
        }
    }
}
