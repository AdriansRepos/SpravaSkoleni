using SpravaSkoleni.Dto;
using SpravaSkoleni.Rozhrani.ImportExport;

namespace SpravaSkoleni.Logika.ImportExport;

public sealed class PdfStudentReader : IFormatStudentReader
{
    public bool PodporujePriponu(string pripona)
        => string.Equals(pripona, ".pdf", StringComparison.OrdinalIgnoreCase);

    public Task<IReadOnlyList<ImportovanyStudentDto>> NactiAsync(string cestaKSouboru)
    {
        // TODO: Doplnit extrakci textu ze stránek (např. přes PdfPig)
        // a parsování řádků na ImportovanyStudentDto(Jmeno, Prijmeni, Kod, PracovniPozice).
        IReadOnlyList<ImportovanyStudentDto> vysledek = [];
        return Task.FromResult(vysledek);
    }
}