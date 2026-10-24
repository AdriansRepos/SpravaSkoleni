using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Zaloha;
using System.Text.Json;

namespace SpravaSkoleni.Data.Repozitare;

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
    /// Uloží seznam školení do JSON souboru (atomicky přes dočasný .tmp soubor).
    /// </summary>
    public static void Ulozit(List<Skoleni> skoleni)
    {
        string json = JsonSerializer.Serialize(skoleni, JsonOptions);

        // Atomický zápis: nejprve do .tmp souboru, pak bezpečné nahrazení
        string docasnySoubor = SouborSkoleni + ".tmp";
        File.WriteAllText(docasnySoubor, json);
        File.Move(docasnySoubor, SouborSkoleni, overwrite: true);

        // atomická záloha
        ZalohovaciSluzba.ProvedAutomatickouZalohu(SouborSkoleni);
    }


    /// <summary>
    /// Načte seznam školení z JSON souboru a propojí kódy na reálné objekty lektorů a studentů.
    /// </summary>
    public static List<Skoleni> Nacist(Func<string, Student?> najdiStudenta, Func<string, Lektor?> najdiLektora)
    {
        if (!File.Exists(SouborSkoleni))
            return [];

        try
        {
            string json = File.ReadAllText(SouborSkoleni);
            var seznam = JsonSerializer.Deserialize<List<Skoleni>>(json, JsonOptions) ?? [];

            foreach (var skoleni in seznam)
            {
                // Obnovení seznamu studentů na základě uložených kódů
                skoleni.Studenti.Clear();
                foreach (var kod in skoleni.KodyStudentu)
                {
                    var student = najdiStudenta(kod);
                    if (student != null)
                        skoleni.Studenti.Add(student);
                }

                // Obnovení seznamu lektorů na základě uložených kódů
                skoleni.Lektori.Clear();
                foreach (var kod in skoleni.KodyLektoru)
                {
                    var lektor = najdiLektora(kod);
                    if (lektor != null)
                        skoleni.Lektori.Add(lektor);
                }

                // --- NOVINKA: obnovit HodnoceniStudentu z persistovaného slovníku Hodnoceni ---
                skoleni.HodnoceniStudentu.Clear();
                if (skoleni.Hodnoceni != null)
                {
                    foreach (var kvp in skoleni.Hodnoceni)
                    {
                        var student = najdiStudenta(kvp.Key);
                        if (student != null)
                            skoleni.HodnoceniStudentu[student] = kvp.Value;
                    }
                }
            }
            return seznam;
        }
        catch (Exception ex)
        {
            VypisyDoKonzole.VypisVarovani($"Chyba při načítání skoleni.json: {ex.Message}");
            return [];
        }
    }
}