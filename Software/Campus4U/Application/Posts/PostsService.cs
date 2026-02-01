using Client.Domain.Posts;

namespace Client.Application.Posts;

//Luka Kanjir
public sealed class PostsService(IPostsRepository postsRepository)
{
    private const int MaxTitleLength = 255;
    private const int MaxBodyLength = 1000;

    public Task<IReadOnlyList<PostListItem>> GetAllAsync(CancellationToken ct = default) =>
        postsRepository.GetAllAsync(ct);

    public async Task<PostSaveResult> CreateAsync(PostCreateRequest request, int authorId,
        CancellationToken ct = default)
    {
        var validation = Validate(request.Naslov, request.Sadrzaj, request.DatumDogadaja);
        if (validation is not null) return new PostSaveResult(false, validation, null);
        if (authorId <= 0) return new PostSaveResult(false, "Neispravan autor.", null);

        var id = await postsRepository.CreateAsync(request, authorId, ct);
        return new PostSaveResult(true, null, id);
    }

    public async Task<PostSaveResult> UpdateAsync(int postId, PostUpdateRequest request, int currentUserId,
        bool isStaff, CancellationToken ct = default)
    {
        if (postId <= 0) return new PostSaveResult(false, "Objava ne postji", null);
        var validation = Validate(request.Naslov, request.Sadrzaj, request.DatumDogadaja);
        if (validation is not null) return new PostSaveResult(false, validation, null);

        var authorId = await postsRepository.GetAuthorIdAsync(postId, ct);
        if (authorId is null) return new PostSaveResult(false, "Objava ne postoji", null);
        if (!isStaff && authorId.Value != currentUserId)
            return new PostSaveResult(false, "Nemate pravo uređivanja objave", null);

        var updated = await postsRepository.UpdateAsync(postId, request, ct);
        return updated
            ? new PostSaveResult(true, null, postId)
            : new PostSaveResult(false, "Greška kod ažuriranja objave", null);
    }

    public async Task<PostDeleteResult> DeleteAsync(int postId, int currentUserId, bool isStaff,
        CancellationToken ct = default)
    {
        if (postId <= 0) return new PostDeleteResult(false, "Objava ne postoji");

        var authorId = await postsRepository.GetAuthorIdAsync(postId, ct);
        if (authorId is null) return new PostDeleteResult(false, "Objava ne postoji");
        if (!isStaff && authorId.Value != currentUserId) return new PostDeleteResult(false, "Nemate pravo brisanja");

        var deleted = await postsRepository.DeleteAsync(postId, ct);
        return deleted ? new PostDeleteResult(true, null) : new PostDeleteResult(false, "Greška kod brisanja objave");
    }

    private static string? Validate(string naslov, string sadrzaj, DateTime datumDogadaja)
    {
        if (string.IsNullOrWhiteSpace(naslov)) return "Naslov je obavezan";
        if (naslov.Length > MaxTitleLength) return $"Naslov je predugačak (max {MaxTitleLength})";
        if (string.IsNullOrWhiteSpace(sadrzaj)) return "Sadrzaj je obavezan";
        if (sadrzaj.Length > MaxBodyLength) return $"Sadrzaj je predugačak (max {MaxBodyLength})";
        if (datumDogadaja == default) return "Datum dogadaj je obavezan";

        return null;
    }
}