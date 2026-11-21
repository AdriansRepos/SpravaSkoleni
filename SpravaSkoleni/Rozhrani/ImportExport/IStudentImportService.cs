using SpravaSkoleni.Dto;

namespace SpravaSkoleni.Rozhrani.ImportExport;

/// <summary>
/// Hlavní vstupní bod nezávislý na formátu souboru i na UI.
/// </summary>
public interface IStudentImportService
{
    Task<IReadOnlyList<ImportovanyStudentDto>> ImportujZeSouboruAsync(string cestaKSouboru);
}
