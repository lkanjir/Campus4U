namespace Client.Application.EventFeedBack;

public sealed record EventFeedbackComment(
    int Id,
    DateTime Datum,
    int Ocjena,
    string Komentar,
    int DogadajId,
    int KorisnikId);
