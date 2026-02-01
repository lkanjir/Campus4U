using Client.Application.Posts;
using Client.Data.Context;
using Client.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Client.Data.Posts;

//Luka Kanjir
public sealed class InterestsRepository : IInterestsRepository
{
    public async Task<bool> IsInterestedAsync(int postId, int userId, CancellationToken ct = default)
    {
        await using var context = new Campus4UContext();
        return await context.DogadajiFavoriti.AnyAsync(d => d.DogadajId == postId && d.KorisnikId == userId, ct);
    }

    public async Task<bool> ToggleAsync(int postId, int userId, CancellationToken ct = default)
    {
        await using var context = new Campus4UContext();
        var existing =
            await context.DogadajiFavoriti.FirstOrDefaultAsync(d => d.DogadajId == postId && d.KorisnikId == userId,
                ct);

        if (existing is null)
        {
            context.DogadajiFavoriti.Add(new DogadajiFavoriti
            {
                DogadajId = postId,
                KorisnikId = userId,
            });
            await context.SaveChangesAsync(ct);
            return true;
        }
        
        context.DogadajiFavoriti.Remove(existing);
        await context.SaveChangesAsync(ct);
        return false;
    }
}