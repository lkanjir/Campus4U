namespace Client.Application.Posts;

//Luka Kanjir
public interface IInterestsRepository
{
    Task<bool> IsInterestedAsync(int postId, int userId, CancellationToken ct = default);
    Task<bool> ToggleAsync(int postId, int userId, CancellationToken ct = default);
}