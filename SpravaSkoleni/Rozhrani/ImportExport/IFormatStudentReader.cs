using SpravaSkoleni.Dto;

namespace SpravaSkoleni.Rozhrani.ImportExport;

/// <summary>
/// Rozhraní pro čtečku konkrétního formátu souboru.
/// </summary>
public interface IFormatStudentReader
{
    bool PodporujePriponu(string pripona);
    Task<IReadOnlyList<ImportovanyStudentDto>> NactiAsync(string cestaKSouboru);
}
