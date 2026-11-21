namespace SpravaSkoleni.Dto;

/// <summary>
/// Jednotný záznam studenta vytažený ze vstupního souboru (PDF, DOCX, XLSX...).
/// </summary>
public sealed record ImportovanyStudentDto(
    string Jmeno,
    string Prijmeni,
    string Kod,
    string PracovniPozice);
