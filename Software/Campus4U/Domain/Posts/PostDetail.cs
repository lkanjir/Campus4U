namespace Client.Domain.Posts;

public sealed record PostDetail(
    int Id,
    string Naslov,
    string Sadrzaj,
    int AutorId,
    string AutorIme,
    DateTime DatumVrijemeObjave,
    DateTime DatumVrijemeDogadaja
);