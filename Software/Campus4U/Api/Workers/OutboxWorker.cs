using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Server.Application.Email;
using Server.Data.Context;

namespace Api.Workers;

public sealed class OutboxWorker(
    IServiceScopeFactory scopeFactory,
    IEmailSender emailSender,
    ILogger<OutboxWorker> logger) : BackgroundService
{
    private static readonly TimeSpan pollingInterval = TimeSpan.FromSeconds(10);
    private const int batchSize = 10;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(pollingInterval);
        while (await timer.WaitForNextTickAsync(ct))
        {
            await ProcessBatchAsync(ct);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Campus4UContext>();

        var pending = await (from o in db.ObavijestiZaSlanje
            where o.Status == "ceka" && o.Dogadjaj == "kvar_prijavljen"
            orderby o.Kreirano
            select o).Take(batchSize).ToListAsync(ct);

        if (pending.Count == 0) return;

        foreach (var obavijest in pending)
        {
            try
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
                    continue;
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