using System.Diagnostics;
using Client.Application.Users;
using Client.Data.Context;
using Client.Data.Entities;
using Client.Domain.Users;
using Microsoft.EntityFrameworkCore;

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
            : new UserProfile(entity.Sub, entity.Email, entity.Ime, entity.Prezime, entity.BrojSobe, entity.UlogaId);
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
                Email = profile.Email,
                BrojSobe = profile.BrojSobe,
            };
            db.Korisnici.Add(entity);
        }
        else
        {
            entity.Email = profile.Email;
            entity.Ime = profile.Ime;
            entity.Prezime = profile.Prezime;
            entity.BrojSobe = profile.BrojSobe;
            entity.UlogaId = profile.UlogaId;
        }
        
        await db.SaveChangesAsync(ct);
    }
}