using Client.Domain.Posts;

namespace Client.Application.Posts;

public interface IPostsRepository
{
    Task<IReadOnlyList<PostListItem>> GetAllAsync(CancellationToken ct = default);
    Task<PostDetail?> GetByIdAsync(int postId, CancellationToken ct = default);
}