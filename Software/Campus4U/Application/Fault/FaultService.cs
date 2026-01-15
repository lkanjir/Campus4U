using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
