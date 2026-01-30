using Microsoft.EntityFrameworkCore;
using Server.Application.Email;
using Server.Data.Context;

namespace Api.Workers;

//Luka Kanjir
public static class Dogadjaji
{
    public const string KvarPrijavljen = "kvar_prijavljen";
    public const string RezervacijaKreirana = "rezervacija_kreirana";
}

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
            using var delayCtSource =  CancellationTokenSource.CreateLinkedTokenSource(ct);
            var delayTask = Task.Delay(pollingInterval, delayCtSource.Token);
            var signalTask = control.WaitForSignalAsync(ct);
            
            var completed = await Task.WhenAny(delayTask, signalTask);
            if(completed == signalTask) delayCtSource.Cancel();
            
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
        var events = new[] { Dogadjaji.KvarPrijavljen, Dogadjaji.RezervacijaKreirana };

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
                        var kvar = await db.Kvarovi
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
}