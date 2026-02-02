using Microsoft.EntityFrameworkCore;
using Server.Application.Email;
using Server.Data.Context;
using Server.Data.Entities;

namespace Api.Workers;

//Luka Kanjir
public sealed class OutboxWorker(
    ITriggerControl control,
    IServiceScopeFactory scopeFactory,
    IEmailSender emailSender,
    ILogger<OutboxWorker> logger) : BackgroundService
{
    private static readonly TimeSpan pollingInterval = TimeSpan.FromSeconds(20);
    private const int batchSize = 20;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            using var delayCtSource = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var delayTask = Task.Delay(pollingInterval, delayCtSource.Token);
            var signalTask = control.WaitForSignalAsync(ct);

            var completed = await Task.WhenAny(delayTask, signalTask);
            if (completed == signalTask) delayCtSource.Cancel();

            if (!control.Enabled) continue;

            try
            {
                await ProcessBatchAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Greška worker-a {greska}", ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Campus4UContext>();
        var events = new[]
        {
            Dogadjaji.KvarPrijavljen, Dogadjaji.RezervacijaKreirana, Dogadjaji.KvarStatusPromijenjen,
            Dogadjaji.RezervacijaStatusPromijenjena, Dogadjaji.DogadajKreiran
        };

        var pending = await (from o in db.ObavijestiZaSlanje
            where o.Status == "ceka" && events.Contains(o.Dogadjaj)
            orderby o.Kreirano
            select o).Take(batchSize).ToListAsync(ct);

        if (pending.Count == 0) return;

        foreach (var obavijest in pending)
        {
            try
            {
                switch (obavijest.Dogadjaj)
                {
                    case Dogadjaji.KvarPrijavljen:
                    {
                        var kvar = await db.Kvarovi.AsNoTracking()
                            .Include(k => k.Prostor)
                            .Include(k => k.VrstaKvara)
                            .Include(k => k.Korisnik)
                            .FirstOrDefaultAsync(k => k.KvarId == obavijest.EntitetId, ct);

                        if (kvar is null)
                        {
                            obavijest.Status = "greska";
                            obavijest.ZadnjaGreska = "Kvar ne postji.";
                            obavijest.Pokusaji++;
                            break;
                        }

                        if (!await IsPreferenceEnabledAsync(db, kvar.KorisnikId, TipoviObavijesti.Kvarovi, ct))
                        {
                            MarkSkipped(obavijest, "Korisnik je iskljucio obavijesti za kvarove.");
                            break;
                        }

                        var subject = $"Obavijest o kvaru: {kvar.Prostor.Naziv}, {kvar.VrstaKvara.Naziv}";
                        var datum = kvar.DatumPrijave.ToString("dd.MM.yyyy.");
                        var vrijeme = kvar.DatumPrijave.ToString("HH:mm");
                        var body = $"""
                                    Poštovana/Poštovani,

                                    obavještavamo Vas da smo zaprimili Vašu prijavu dana {datum} u {vrijeme} sati,
                                    glede prijave kvara: {subject}.

                                    Kvar ćemo nastojati ukloniti u najkraćem mogućem roku.

                                    Srdačan pozdrav,
                                    Campus4U
                                    """;

                        await emailSender.SendAsync(new EmailMessage
                        {
                            ToEmail = kvar.Korisnik.Email,
                            ToName = $"{kvar.Korisnik.Ime} {kvar.Korisnik.Prezime}".Trim(),
                            Subject = subject,
                            Body = body
                        }, ct);

                        obavijest.Status = "poslano";
                        obavijest.ZadnjaGreska = null;
                        break;
                    }

                    case Dogadjaji.RezervacijaKreirana:
                    {
                        var rez = await db.Rezervacije
                            .AsNoTracking()
                            .Include(r => r.Prostor)
                            .Include(r => r.Korisnik)
                            .FirstOrDefaultAsync(r => r.Id == obavijest.EntitetId, ct);

                        if (rez is null)
                        {
                            obavijest.Status = "greska";
                            obavijest.ZadnjaGreska = "Rezervacija ne postoji.";
                            obavijest.Pokusaji++;
                            break;
                        }

                        if (!await IsPreferenceEnabledAsync(db, rez.KorisnikId, TipoviObavijesti.Rezervacije, ct))
                        {
                            MarkSkipped(obavijest, "Korisnik je iskljucio obavijesti za rezervacije.");
                            break;
                        }

                        var subject = $"Potvrda rezervacije: {rez.Prostor.Naziv}";
                        var datum = rez.VrijemeOd.ToString("dd.MM.yyyy.");
                        var vrijemeOd = rez.VrijemeOd.ToString("HH:mm");
                        var vrijemeDo = rez.VrijemeDo.ToString("HH:mm");
                        var body = $"""
                                    Poštovana/Poštovani,

                                    obavještavamo Vas da ste uspješno rezervirali {rez.Prostor.Naziv}
                                    u terminu od {vrijemeOd} do {vrijemeDo} sati.
                                    Datum rezervacije: {datum}

                                    Srdačan pozdrav,
                                    Campus4U
                                    """;

                        await emailSender.SendAsync(new EmailMessage
                        {
                            ToEmail = rez.Korisnik.Email,
                            ToName = $"{rez.Korisnik.Ime} {rez.Korisnik.Prezime}".Trim(),
                            Subject = subject,
                            Body = body
                        }, ct);

                        obavijest.Status = "poslano";
                        obavijest.ZadnjaGreska = null;
                        break;
                    }

                    case Dogadjaji.KvarStatusPromijenjen:
                    {
                        var kvar = await db.Kvarovi
                            .AsNoTracking()
                            .Include(k => k.Prostor)
                            .Include(k => k.VrstaKvara)
                            .Include(k => k.Korisnik)
                            .FirstOrDefaultAsync(k => k.KvarId == obavijest.EntitetId, ct);

                        if (kvar is null)
                        {
                            obavijest.Status = "greska";
                            obavijest.ZadnjaGreska = "Kvar ne postji.";
                            obavijest.Pokusaji++;
                            break;
                        }

                        if (!await IsPreferenceEnabledAsync(db, kvar.KorisnikId, TipoviObavijesti.Kvarovi, ct))
                        {
                            MarkSkipped(obavijest, "Korisnik je iskljucio obavijesti za kvarove.");
                            break;
                        }

                        var subject = $"Promjena statusa kvara: {kvar.Prostor.Naziv}, {kvar.VrstaKvara.Naziv}";
                        var datum = DateTime.Now.ToString("dd.MM.yyyy.");
                        var vrijeme = DateTime.Now.ToString("HH:mm");
                        var status = kvar.Status;
                        var body = $"""
                                    Poštovana/Poštovani,

                                    obavještavamo Vas da je na {datum} u {vrijeme} sati promijenjen status
                                    Vaše prijave kvara za prostor: {kvar.Prostor.Naziv}.

                                    Novi status: {status}

                                    Srdačan pozdrav,
                                    Campus4U
                                    """;

                        await emailSender.SendAsync(new EmailMessage
                        {
                            ToEmail = kvar.Korisnik.Email,
                            ToName = $"{kvar.Korisnik.Ime} {kvar.Korisnik.Prezime}".Trim(),
                            Subject = subject,
                            Body = body
                        }, ct);

                        obavijest.Status = "poslano";
                        obavijest.ZadnjaGreska = null;
                        break;
                    }

                    case Dogadjaji.RezervacijaStatusPromijenjena:
                    {
                        var rez = await db.Rezervacije
                            .AsNoTracking()
                            .Include(r => r.Prostor)
                            .Include(r => r.Korisnik)
                            .FirstOrDefaultAsync(r => r.Id == obavijest.EntitetId, ct);

                        if (rez is null)
                        {
                            obavijest.Status = "greska";
                            obavijest.ZadnjaGreska = "Rezervacija ne postoji.";
                            obavijest.Pokusaji++;
                            break;
                        }

                        if (!await IsPreferenceEnabledAsync(db, rez.KorisnikId, TipoviObavijesti.Rezervacije, ct))
                        {
                            MarkSkipped(obavijest, "Korisnik je iskljucio obavijesti za rezervacije.");
                            break;
                        }

                        var subject = $"Promjena statusa rezervacije: {rez.Prostor.Naziv}";
                        var datum = rez.VrijemeOd.ToString("dd.MM.yyyy.");
                        var vrijemeOd = rez.VrijemeOd.ToString("HH:mm");
                        var vrijemeDo = rez.VrijemeDo.ToString("HH:mm");
                        var status = rez.Status;
                        var body = $"""
                                    Poštovana/Poštovani,

                                    obavještavamo Vas da je promijenjen status Vaše rezervacije prostora {rez.Prostor.Naziv}
                                    za datum {datum} u terminu od {vrijemeOd} do {vrijemeDo} sati.

                                    Novi status: {status}

                                    Srdačan pozdrav,
                                    Campus4U
                                    """;

                        await emailSender.SendAsync(new EmailMessage
                        {
                            ToEmail = rez.Korisnik.Email,
                            ToName = $"{rez.Korisnik.Ime} {rez.Korisnik.Prezime}".Trim(),
                            Subject = subject,
                            Body = body
                        }, ct);

                        obavijest.Status = "poslano";
                        obavijest.ZadnjaGreska = null;
                        break;
                    }

                    case Dogadjaji.DogadajKreiran:
                    {
                        var dogadjaj = await db.Dogadaji
                            .AsNoTracking()
                            .FirstOrDefaultAsync(d => d.Id == obavijest.EntitetId, ct);

                        if (dogadjaj is null)
                        {
                            obavijest.Status = "greska";
                            obavijest.ZadnjaGreska = "Dogadjaj ne postoji.";
                            obavijest.Pokusaji++;
                            break;
                        }

                        var recipients = await (from k in db.Korisnici
                            join u in db.Uloge on k.UlogaId equals u.Id
                            where u.NazivUloge == "student"
                            select new { k.Id, k.Email, k.Ime, k.Prezime }).ToListAsync(ct);

                        if (recipients.Count == 0)
                        {
                            obavijest.Status = "poslano";
                            obavijest.ZadnjaGreska = null;
                            break;
                        }

                        var recipientIds = recipients.Select(r => r.Id).ToArray();
                        var preferences = await db.ObavijestiPostavke
                            .AsNoTracking()
                            .Where(p => p.Tip == TipoviObavijesti.Dogadjaji && recipientIds.Contains(p.KorisnikId))
                            .ToDictionaryAsync(p => p.KorisnikId, p => p.Omoguceno, ct);

                        var subject = $"Nova objava u oglasniku: {dogadjaj.Naslov}";
                        var datumObjave = dogadjaj.VrijemeObjave.ToString("dd.MM.yyyy.");
                        var datumDogadaja = dogadjaj.VrijemeDogadaja.ToString("dd.MM.yyyy.");
                        var vrijemeDogadaja = dogadjaj.VrijemeDogadaja.ToString("HH:mm");

                        foreach (var recipient in recipients)
                        {
                            if (string.IsNullOrWhiteSpace(recipient.Email)) continue;
                            if (preferences.TryGetValue(recipient.Id, out var enabled) && !enabled) continue;

                            var body = $"""
                                        Poštovana/Poštovani,

                                        objavljena je nova obavijest u oglasniku.
                                        Naslov: {dogadjaj.Naslov}
                                        Datum objave: {datumObjave}

                                        Datum događaja: {datumDogadaja} u {vrijemeDogadaja} sati.

                                        Srdačan pozdrav,
                                        Campus4U
                                        """;

                            await emailSender.SendAsync(new EmailMessage
                            {
                                ToEmail = recipient.Email,
                                ToName = $"{recipient.Ime} {recipient.Prezime}".Trim(),
                                Subject = subject,
                                Body = body
                            }, ct);
                        }

                        obavijest.Status = "poslano";
                        obavijest.ZadnjaGreska = null;
                        break;
                    }

                    default:
                    {
                        obavijest.Status = "greska";
                        obavijest.ZadnjaGreska = $"Nepoznat dogadjaj: {obavijest.Dogadjaj}";
                        obavijest.Pokusaji++;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                obavijest.Status = "greska";
                obavijest.ZadnjaGreska = ex.Message;
                obavijest.Pokusaji++;
                logger.LogError(ex, "Slanje nije uspjelo za obavijest {id}", obavijest.ObavijestId);
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private void MarkSkipped(ObavijestiZaSlanje obavijest, string razlog)
    {
        obavijest.Status = "preskoceno";
        obavijest.ZadnjaGreska = razlog;
    }

    private async Task<bool> IsPreferenceEnabledAsync(Campus4UContext db, int korisnikId, string tip,
        CancellationToken ct)
    {
        var pref = await db.ObavijestiPostavke.AsNoTracking()
            .FirstOrDefaultAsync(p => p.KorisnikId == korisnikId && p.Tip == tip, ct);
        return pref?.Omoguceno ?? true;
    }
}