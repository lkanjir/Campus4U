using Client.Application.Spaces;
using Client.Domain.Fault;
using Client.Domain.Spaces;

//Tin Posavec

namespace Client.Application.Fault
{
    public sealed class FaultService : IFaultService
    {
        private readonly IFaultRepository _kvarRepo;
        private readonly ISpaceRepository _prostorRepo;

        public FaultService(IFaultRepository kvarRepo, ISpaceRepository prostorRepo)
        {
            _kvarRepo = kvarRepo;
            _prostorRepo = prostorRepo;
        }

        public async Task<List<Space>> DohvatiProstore()
        {
            return await _prostorRepo.DohvatiSveProstore();
        }

        public async Task<List<FaultType>> DohvatiVrsteKvarova()
        {
            return await _kvarRepo.DohvatiVrsteKvarova();
        }

        public async Task<bool> PrijaviKvar(int prostorId, int vrstaKvaraId, string opis, byte[]? fotografija, int korisnikId)
        {
            if (prostorId <= 0)
                throw new ArgumentException("Prostor mora biti odabran.", nameof(prostorId));

            if (vrstaKvaraId <= 0)
                throw new ArgumentException("Vrsta kvara mora biti odabrana.", nameof(vrstaKvaraId));

            if (string.IsNullOrWhiteSpace(opis))
                throw new ArgumentException("Opis kvara je obavezan.", nameof(opis));

            if (korisnikId <= 0)
                throw new ArgumentException("Korisnik nije validan.", nameof(korisnikId));

            var kvar = new FaultReport(
                KvarId: 0,
                KorisnikId: korisnikId,
                ProstorId: prostorId,
                VrstaKvaraId: vrstaKvaraId,
                Opis: opis,
                Fotografija: fotografija,
                Status: "Aktivan",
                DatumPrijave: DateTime.Now
            );

            return await _kvarRepo.SpremiKvar(kvar);
        }

        // FZ-11
        private static readonly List<string> _sviStatusi = new() { "Aktivan", "U obradi", "Riješen" };

        public async Task<List<FaultReport>> DohvatiSveKvarove()
        {
            return await _kvarRepo.DohvatiSveKvarove();
        }

        public async Task<FaultReport?> DohvatiKvarPoId(int kvarId)
        {
            if (kvarId <= 0)
                throw new ArgumentException("Neispravan ID kvara.", nameof(kvarId));

            return await _kvarRepo.DohvatiKvarPoId(kvarId);
        }

        public async Task<List<FaultReport>> DohvatiKvaroveFiltrirano(string? status, int? prostorId, int? vrstaKvaraId, DateTime? odDatuma, DateTime? doDatuma)
        {
            if (odDatuma.HasValue && doDatuma.HasValue && odDatuma > doDatuma)
                throw new ArgumentException("Datum 'od' ne može biti veći od datuma 'do'.");

            return await _kvarRepo.DohvatiKvaroveFiltrirano(status, prostorId, vrstaKvaraId, odDatuma, doDatuma);
        }

        public async Task<bool> PromijeniStatusKvara(int kvarId, string noviStatus)
        {
            if (kvarId <= 0)
                throw new ArgumentException("Neispravan ID kvara.", nameof(kvarId));

            if (string.IsNullOrWhiteSpace(noviStatus))
                throw new ArgumentException("Status ne može biti prazan.", nameof(noviStatus));

            var kvar = await _kvarRepo.DohvatiKvarPoId(kvarId);
            if (kvar == null)
                throw new ArgumentException("Kvar ne postoji.", nameof(kvarId));

            var dozvoljeni = DohvatiDozvoljeneStatuse(kvar.Status);
            if (!dozvoljeni.Contains(noviStatus))
                throw new InvalidOperationException($"Nije dozvoljen prijelaz iz '{kvar.Status}' u '{noviStatus}'.");

            return await _kvarRepo.AzurirajStatusKvara(kvarId, noviStatus);
        }

        public List<string> DohvatiSveStatuse()
        {
            return _sviStatusi;
        }

        public List<string> DohvatiDozvoljeneStatuse(string trenutniStatus)
        {
            return trenutniStatus switch
            {
                "Aktivan" => new List<string> { "U obradi", "Riješen" },
                "U obradi" => new List<string> { "Riješen" },
                "Riješen" => new List<string>(),
                _ => new List<string>()
            };
        }

        public async Task<FaultStatistics> DohvatiStatistiku()
        {
            return await _kvarRepo.DohvatiStatistiku();
        }
    }
}
