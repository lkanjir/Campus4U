namespace Server.Application.Storage;

//Luka Kanjir
public sealed class StorageOptions
{
    public required string RootPath { get; init; }
    public required long MaxBytes { get; init; }
    public required string[] AllowedTypes { get; init; }
}