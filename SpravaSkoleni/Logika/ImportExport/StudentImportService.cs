using SpravaSkoleni.Dto;
using SpravaSkoleni.Rozhrani.ImportExport;

namespace SpravaSkoleni.Logika.ImportExport;

public sealed class StudentImportService(IEnumerable<IFormatStudentReader> ctecky) : IStudentImportService
{
    private readonly IReadOnlyList<IFormatStudentReader> _ctecky = [.. ctecky];

    public async Task<IReadOnlyList<ImportovanyStudentDto>> ImportujZeSouboruAsync(string cestaKSouboru)
    {
        if (string.IsNullOrWhiteSpace(cestaKSouboru) || !File.Exists(cestaKSouboru))
        {
            throw new FileNotFoundException("Vstupní soubor se seznamu studentů nebyl nalezen.", cestaKSouboru);
        }

        string pripona = Path.GetExtension(cestaKSouboru).ToLowerInvariant();
        var ctecka = _ctecky.FirstOrDefault(r => r.PodporujePriponu(pripona))
            ?? throw new NotSupportedException($"Formát '{pripona}' zatím není podporován.");

        var zaznamy = await ctecka.NactiAsync(cestaKSouboru);

        // Očištění a kontrola povinných položek (jméno, příjmení, kód, funkce)
        return [.. zaznamy
            .Where(z =>
                !string.IsNullOrWhiteSpace(z.Jmeno) &&
                !string.IsNullOrWhiteSpace(z.Prijmeni) &&
                !string.IsNullOrWhiteSpace(z.Kod))
            .Select(z => new ImportovanyStudentDto(
                z.Jmeno.Trim(),
                z.Prijmeni.Trim(),
                z.Kod.Trim(),
                string.IsNullOrWhiteSpace(z.PracovniPozice) ? "Neuvedeno" : z.PracovniPozice.Trim()))];
    }
}