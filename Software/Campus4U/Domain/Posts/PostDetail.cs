namespace Client.Domain.Posts;

public sealed class PostDetail(
    int Id,
    string Naslov,
    string Sadrzaj,
    string AutorIme,
    DateTime DatumVrijemeObjave,
    DateTime DatumVrijemeDogadaja
);