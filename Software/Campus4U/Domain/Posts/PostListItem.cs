namespace Client.Domain.Posts;

public sealed record PostListItem(
    int Id,
    string Naslov,
    string AutorIme,
    DateTime DatumVrijemeObjave,
    DateTime DatumVrijemeDogadaja
);