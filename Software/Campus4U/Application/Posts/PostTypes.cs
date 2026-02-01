namespace Client.Application.Posts;

//Luka Kanjir
public sealed record PostCreateRequest(string Naslov, string Sadrzaj, DateTime DatumDogadaja);

//Luka Kanjir
public sealed record PostUpdateRequest(string Naslov, string Sadrzaj, DateTime DatumDogadaja);

//Luka Kanjir
public sealed record PostSaveResult(bool IsSuccess, string? Error, int? PostId);

//Luka Kanjir
public sealed record PostDeleteResult(bool IsSuccess, string? Error);