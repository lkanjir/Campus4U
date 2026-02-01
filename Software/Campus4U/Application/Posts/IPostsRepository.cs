using Client.Domain.Posts;

namespace Client.Application.Posts;

//Luka Kanjir
public interface IPostsRepository
{
    Task<IReadOnlyList<PostListItem>> GetAllAsync(CancellationToken ct = default);
    Task<PostDetail?> GetByIdAsync(int postId, CancellationToken ct = default);
    Task<int?> GetAuthorIdAsync(int postId, CancellationToken ct = default);
    Task<int> CreateAsync(PostCreateRequest request, int authorId, CancellationToken ct = default);
    Task<bool> UpdateAsync(int postId, PostUpdateRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int postId, CancellationToken ct = default);
    Task<DateTime?> GetEventDateAsync(int postId, CancellationToken ct = default);
}