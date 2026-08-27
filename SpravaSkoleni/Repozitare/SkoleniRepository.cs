using SpravaSkoleni.Entity;
using System.Text.Json;
using BarevneVypisyHelper;

namespace SpravaSkoleni.Repozitare;

/// <summary>
/// Datová vrstva pro práci se souborem skoleni.json.
/// </summary>
public class SkoleniRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private const string SouborSkoleni = "skoleni.json";

    /// <summary>
    /// Uloží seznam školení do JSON souboru (napáruje kódy studentů a lektorů).
    /// </summary>
    public static void Ulozit(List<Skoleni> seznamSkoleni)
    {
        List<Skoleni> kUlozeni = [];

        foreach (var skoleni in seznamSkoleni)
        {
            skoleni.KodyStudentu = [.. skoleni.Studenti.Select(s => s.Kod!)];
            skoleni.KodyLektoru = [.. skoleni.Lektori.Select(l => l.Kod!)];
            skoleni.Hodnoceni = skoleni.HodnoceniStudentu
                .ToDictionary(kv => kv.Key.Kod!, kv => kv.Value);

            kUlozeni.Add(skoleni);
        }

        string json = JsonSerializer.Serialize(kUlozeni, JsonOptions);
        File.WriteAllText(SouborSkoleni, json);
    }

    /// <summary>
    /// Načte školení z JSON a dopojí reference na objekty Studentů a Lektorů.
    /// </summary>
    public static List<Skoleni> Nacist(Func<string, Student?> najdiStudenta, Func<string, Lektor?> najdiLektora)
    {
        if (!File.Exists(SouborSkoleni))
            return [];

        try
        {
            string json = File.ReadAllText(SouborSkoleni);
            var nactena = JsonSerializer.Deserialize<List<Skoleni>>(json, JsonOptions) ?? [];

            foreach (var skoleni in nactena)
            {
                skoleni.Studenti = [.. skoleni.KodyStudentu
                    .Select(kod => najdiStudenta(kod))
                    .Where(s => s != null)
                    .Select(s => s!)];

                skoleni.Lektori = [.. skoleni.KodyLektoru
                    .Select(kod => najdiLektora(kod))
                    .Where(l => l != null)
                    .Select(l => l!)];

                skoleni.HodnoceniStudentu = skoleni.Hodnoceni
                    .Where(kv => najdiStudenta(kv.Key) != null)
                    .ToDictionary(kv => najdiStudenta(kv.Key)!, kv => kv.Value);

                foreach (var student in skoleni.Studenti)
                    student.Skoleni.Add(skoleni);

                foreach (var lektor in skoleni.Lektori)
                    lektor.VyucovanaSkoleni.Add(skoleni);
            }

            return nactena;
        }
        catch (Exception ex)
        {           
            VypisyDoKonzole.VypisVarovani($"Chyba při načítání skoleni.json: {ex.Message}");            
            return [];
        }
    }
}