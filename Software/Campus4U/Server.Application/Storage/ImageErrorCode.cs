namespace Server.Application.Storage;

public enum ImageErrorCode
{
    NotFound,
    Unauthorized,
    Forbidden,
    Invalid,
    TooLarge,
    UnsupportedType,
    StorageFailure
}