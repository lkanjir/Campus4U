namespace Client.Application.EventFeedBack;
/// Nikola Kihas
public sealed record EventFeedbackComment(
    int Id,
    DateTime Datum,
    int Ocjena,
    string Komentar,
    string ImePrezime,
    bool MojKomentar,
    int DogadajId,
    int KorisnikId);
