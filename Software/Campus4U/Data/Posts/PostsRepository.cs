using Client.Application.Posts;
using Client.Data.Context;
using Client.Data.Context.Entities;
using Client.Domain.Posts;
using Microsoft.EntityFrameworkCore;

namespace Client.Data.Posts;

//Luka Kanjir
public sealed class PostsRepository : IPostsRepository
{
    public async Task<IReadOnlyList<PostListItem>> GetAllAsync(CancellationToken ct = default)
    {
        await using var context = new Campus4UContext();
        return await (from d in context.Dogadaji.AsNoTracking()
            orderby d.VrijemeObjave descending
            select new PostListItem(d.Id, d.Naslov,
                ((d.Autor.Ime ?? string.Empty) + " " + (d.Autor.Prezime ?? string.Empty)).Trim(), d.VrijemeObjave,
                d.VrijemeDogadaja)).ToArrayAsync(ct);
    }

    public async Task<PostDetail?> GetByIdAsync(int postId, CancellationToken ct = default)
    {
        await using var context = new Campus4UContext();
        return await (from d in context.Dogadaji.AsNoTracking()
            where d.Id == postId
            select new PostDetail(d.Id, d.Naslov, d.Opis, d.AutorId,
                ((d.Autor.Ime ?? string.Empty) + " " + (d.Autor.Prezime ?? string.Empty)).Trim(), d.VrijemeObjave,
                d.VrijemeDogadaja)).FirstOrDefaultAsync(ct);
    }

    public async Task<int?> GetAuthorIdAsync(int postId, CancellationToken ct = default)
    {
        await using var context = new Campus4UContext();
        return await (from d in context.Dogadaji where d.Id == postId select (int?)d.AutorId).FirstOrDefaultAsync(ct);
    }

    public async Task<int> CreateAsync(PostCreateRequest request, int authorId, CancellationToken ct = default)
    {
        await using var context = new Campus4UContext();
        var entity = new Dogadaji
        {
            Naslov = request.Naslov.Trim(),
            Opis = request.Sadrzaj.Trim(),
            VrijemeObjave = DateTime.Now,
            VrijemeDogadaja = request.DatumDogadaja,
            AutorId = authorId,
            Slika = []
        };

        context.Dogadaji.Add(entity);
        await context.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(int postId, PostUpdateRequest request, CancellationToken ct = default)
    {
        await using var context = new Campus4UContext();
        var entity = await context.Dogadaji.FirstOrDefaultAsync(d => d.Id == postId, ct);
        if (entity is null) return false;

        entity.Naslov = request.Naslov.Trim();
        entity.Opis = request.Sadrzaj.Trim();
        entity.VrijemeDogadaja = request.DatumDogadaja;

        return await context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(int postId, CancellationToken ct = default)
    {
        await using var context = new Campus4UContext();
        var entity = await context.Dogadaji.FirstOrDefaultAsync(d => d.Id == postId, ct);
        if (entity is null) return false;

        context.Dogadaji.Remove(entity);
        return await context.SaveChangesAsync(ct) > 0;
    }

    public async Task<DateTime?> GetEventDateAsync(int postId, CancellationToken ct = default)
    {
        await using var context = new Campus4UContext();
        return await (from d in context.Dogadaji where d.Id == postId select (DateTime?)d.VrijemeDogadaja)
            .FirstOrDefaultAsync(ct);
    }
}