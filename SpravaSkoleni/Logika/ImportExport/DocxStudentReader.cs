using SpravaSkoleni.Dto;
using SpravaSkoleni.Rozhrani.ImportExport;

namespace SpravaSkoleni.Logika.ImportExport;

public sealed class DocxStudentReader : IFormatStudentReader
{
    public bool PodporujePriponu(string pripona)
        => string.Equals(pripona, ".docx", StringComparison.OrdinalIgnoreCase);

    public Task<IReadOnlyList<ImportovanyStudentDto>> NactiAsync(string cestaKSouboru)
    {
        // TODO: Doplnit čtení tabulek / odstavců přes DocumentFormat.OpenXml
        // a mapování na ImportovanyStudentDto(Jmeno, Prijmeni, Kod, PracovniPozice).
        IReadOnlyList<ImportovanyStudentDto> vysledek = [];
        return Task.FromResult(vysledek);
    }
}