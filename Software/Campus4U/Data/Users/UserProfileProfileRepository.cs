using System.Diagnostics;
using Client.Application.Users;
using Client.Data.Context;
using Client.Data.Entities;
using Client.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Client.Data.Users;

//Luka Kanjir
public class UserProfileProfileRepository : IUserProfileRepository
{
    public async Task<UserProfile?> GetBySubAsync(string sub, CancellationToken ct = default)
    {
        await using var db = new Campus4UContext();
        var entity = await
            (from u in db.Korisnici.AsNoTracking()
                where u.Sub == sub
                select u).FirstOrDefaultAsync(ct);

        return entity is null
            ? null
            : new UserProfile(entity.Id, entity.Sub, entity.Email, entity.Ime, entity.Prezime, entity.KorisnickoIme, entity.BrojSobe, entity.BrojTelefona, entity.SlikaProfila, entity.UlogaId);
    }

    public async Task<int?> GetRoleIdByNameAsync(string roleName, CancellationToken ct = default)
    {
        var normalized = roleName.Trim().ToLowerInvariant();
        await using var db = new Campus4UContext();
        var entity = await
            (from u in db.Uloge.AsNoTracking()
                where u.NazivUloge == normalized
                select (int?)u.Id).FirstOrDefaultAsync(ct);

        Debug.WriteLine($"Role id iz baze: {entity}");
        return entity;
    }
    public async Task SaveAsync(UserProfile profile, CancellationToken ct = default)
    {
        await using var db = new Campus4UContext();
        var entity = await (from u in db.Korisnici
            where u.Sub == profile.Sub
            select u).FirstOrDefaultAsync(ct);

        if (entity is null)
        {
            entity = new Korisnici
            {
                UlogaId = profile.UlogaId,
                Sub = profile.Sub,
                Ime = profile.Ime,
                Prezime = profile.Prezime,
                KorisnickoIme = profile.KorisnickoIme,
                Email = profile.Email,
                BrojSobe = profile.BrojSobe,
                BrojTelefona = profile.BrojTelefona,
                SlikaProfila = profile.SlikaProfila
            };
            db.Korisnici.Add(entity);
        }
        else
        {
            entity.Email = profile.Email;
            entity.Ime = profile.Ime;
            entity.Prezime = profile.Prezime;
            entity.KorisnickoIme = profile.KorisnickoIme;
            entity.BrojSobe = profile.BrojSobe;
            entity.UlogaId = profile.UlogaId;
            entity.BrojTelefona = profile.BrojTelefona;
            entity.SlikaProfila = profile.SlikaProfila;
        }
        
        await db.SaveChangesAsync(ct);
    }
    /// Nikola Kihas
    public async Task<bool> AzurirajKorisnikaAsync(UserProfile profile)
    {
        await using var db = new Campus4UContext();
        var entity = await (from u in db.Korisnici
                            where u.Id == profile.Id
                            select u).FirstOrDefaultAsync();
        if (entity is null)
        {
            return false;
        }

        entity.Ime = profile.Ime;
        entity.Prezime = profile.Prezime;
        entity.KorisnickoIme = profile.KorisnickoIme;
        entity.BrojSobe = profile.BrojSobe;
        entity.UlogaId = profile.UlogaId;
        entity.BrojTelefona = profile.BrojTelefona;
        entity.SlikaProfila = profile.SlikaProfila;

        var changed = await db.SaveChangesAsync();
        return changed > 0;
    }
    /// Nikola Kihas
    public Task<bool> AzurirajProfilnuSlikuAsync(int id, string urlSlike)
    {
        // Implementacija æe se napraviti kada server bude spreman
        throw new NotImplementedException();
    }

}