namespace Client.Domain.Posts;

public sealed class PostDetail(
    int Id,
    string Naslov,
    string Sadrzaj,
    int AutorId,
    string AutorIme,
    DateTime DatumVrijemeObjave,
    DateTime DatumVrijemeDogadaja
);