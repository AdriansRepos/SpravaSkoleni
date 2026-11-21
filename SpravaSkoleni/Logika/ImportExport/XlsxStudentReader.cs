using SpravaSkoleni.Dto;
using SpravaSkoleni.Rozhrani.ImportExport;

namespace SpravaSkoleni.Logika.ImportExport;

public sealed class XlsxStudentReader : IFormatStudentReader
{
    public bool PodporujePriponu(string pripona)
        => string.Equals(pripona, ".xlsx", StringComparison.OrdinalIgnoreCase);

    public Task<IReadOnlyList<ImportovanyStudentDto>> NactiAsync(string cestaKSouboru)
    {
        // TODO: Doplnit čtení řádků z listu (např. přes ClosedXML)
        // a mapování na ImportovanyStudentDto(Jmeno, Prijmeni, Kod, PracovniPozice).
        IReadOnlyList<ImportovanyStudentDto> vysledek = [];
        return Task.FromResult(vysledek);
    }
}