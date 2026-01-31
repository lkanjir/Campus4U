using Client.Application.Posts;
using Client.Data.Context;
using Client.Domain.Posts;
using Microsoft.EntityFrameworkCore;

namespace Client.Data.Posts;

public sealed class PostRepository : IPostsRepository
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
            select new PostDetail(d.Id, d.Naslov, d.Opis,
                ((d.Autor.Ime ?? string.Empty) + " " + (d.Autor.Prezime ?? string.Empty)).Trim(), d.VrijemeObjave,
                d.VrijemeDogadaja)).FirstOrDefaultAsync(ct);
    }
}