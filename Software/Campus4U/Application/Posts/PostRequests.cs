namespace Client.Application.Posts;

//Luka Kanjir
public sealed record PostCreateRequest(string Naslov, string Sadrzaj, DateTime DatumDogadaja);

//Luka Kanjir
public sealed record PostUpdateRequest(string Naslov, string Sadrzaj, DateTime DatumDogadaja);