namespace Server.Application.Storage;

public sealed class ImageException : Exception
{
    public ImageErrorCode Code { get; }

    public ImageException(ImageErrorCode code, string message) : base(message)
    {
        Code = code;
    }
}