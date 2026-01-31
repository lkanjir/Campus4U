namespace Client.Application.Images;

//Luka Kanjir
public sealed record ImagePayload(byte[] Bytes, string ContentType);

//Luka Kanjir
public sealed record ImageKey(ImageType Type, int id);

//Luka Kanjir
public enum ImageType
{
    Profile,
    Event,
    Fault
}