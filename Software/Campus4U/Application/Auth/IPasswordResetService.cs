namespace Client.Application.Auth;

public interface IPasswordResetService
{
    Task<PasswordResetResult> SendPasswordResetEmailAsync(string email, CancellationToken ct = default);
}