using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Zaloha;
using System.Text.Json;

namespace SpravaSkoleni.Data.Repozitare;

/// <summary>
/// Datová vrstva pro práci se souborem skoleni.json.
/// </summary>
public class SkoleniRepository(string slozkaSkoleni = "Skoleni")
{
    private readonly string _slozkaSkoleni = slozkaSkoleni;
    private readonly string _cestaKSouboru = Path.Combine(slozkaSkoleni, "skoleni.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public List<Skoleni> Nacist()
    {
        // Pokud chybí hlavní soubor, zkusíme tichou obnovu ze zálohy
        string cestaKZaloze = Path.Combine("Zalohy", "skoleni.bak.json");
        if (!File.Exists(_cestaKSouboru) && File.Exists(cestaKZaloze))
        {
            try
            {
                Directory.CreateDirectory(_slozkaSkoleni);
                File.Copy(cestaKZaloze, _cestaKSouboru);
            }
            catch { /* Defenzivní skip */ }
        }

        if (!File.Exists(_cestaKSouboru)) return [];

        try
        {
            string json = File.ReadAllText(_cestaKSouboru);
            return JsonSerializer.Deserialize<List<Skoleni>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public void Ulozit(List<Skoleni> seznamSkoleni)
    {
        // Auto-generování složky na disku (All-in-One princip)
        Directory.CreateDirectory(_slozkaSkoleni);

        string json = JsonSerializer.Serialize(seznamSkoleni, JsonOptions);

        // Neprůstřelný crash-safe zápis přes dočasný soubor
        string docasnySoubor = _cestaKSouboru + ".tmp";
        File.WriteAllText(docasnySoubor, json);
        File.Move(docasnySoubor, _cestaKSouboru, overwrite: true);

        // Automatická tichá záloha na pozadí
        ZalohovaciSluzba.ProvedAutomatickouZalohu(_cestaKSouboru);
    }
}