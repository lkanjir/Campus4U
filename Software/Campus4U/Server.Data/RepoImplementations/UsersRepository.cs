using Microsoft.EntityFrameworkCore;
using Server.Application.Repositories;
using Server.Data.Context;

namespace Server.Data.RepoImplementations;

public sealed class UsersRepository(Campus4UContext db) : IUsersRepository
{
    private const string StaffRoleName = "osoblje";

    public async Task<UserAuthInfo?> GetBySubAsync(string sub, CancellationToken ct = default)
    {
        return await (from k in db.Korisnici.AsNoTracking()
                where k.Sub == sub
                select new UserAuthInfo(k.Id, k.Uloga.NazivUloge == StaffRoleName))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> SetProfileImagePathAsync(int userId, string imagePath, CancellationToken ct = default)
    {
        var user = await db.Korisnici.FirstOrDefaultAsync(k => k.Id == userId, ct);
        if (user is null) return false;

        user.SlikaPutanja = imagePath;
        await db.SaveChangesAsync(ct);
        return true;
    }
}