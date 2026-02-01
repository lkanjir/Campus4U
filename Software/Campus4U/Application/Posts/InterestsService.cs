namespace Client.Application.Posts;

//Luka Kanjir
public sealed class InterestsService(IInterestsRepository interestsRepository, IPostsRepository postsRepository)
{
    public async Task<bool> IsInterestedAsync(int postId, int userId, CancellationToken ct = default)
    {
        if (postId <= 0 || userId <= 0) return false;
        return await interestsRepository.IsInterestedAsync(postId, userId, ct);
    }

    public async Task<InterestToggleResult> ToggleAsync(int postId, int userId, CancellationToken ct = default)
    {
        if (postId <= 0) return new InterestToggleResult(false, false, "Objava ne postoji");
        if (userId <= 0) return new InterestToggleResult(false, false, "Korisnik ne postoji");

        var eventDate = await postsRepository.GetEventDateAsync(postId, ct);
        if (eventDate is null) return new InterestToggleResult(false, false, "Objava ne postoji");
        if (eventDate.Value == default) return new InterestToggleResult(false, false, "Neispravan datum događaja");

        var isInterested = await interestsRepository.ToggleAsync(postId, userId, ct);
        return new InterestToggleResult(true, isInterested, null);
    }
}