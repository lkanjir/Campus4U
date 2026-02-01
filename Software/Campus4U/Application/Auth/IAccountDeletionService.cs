namespace Client.Application.Auth;

public interface IAccountDeletionService
{
    Task<AccountDeletionResult> DeleteAccountAsync(string userId, CancellationToken ct = default);
}
