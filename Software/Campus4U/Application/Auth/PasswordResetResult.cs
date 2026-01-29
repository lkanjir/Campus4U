namespace Client.Application.Auth;

public sealed record PasswordResetResult(bool IsSuccess, string? Error);
