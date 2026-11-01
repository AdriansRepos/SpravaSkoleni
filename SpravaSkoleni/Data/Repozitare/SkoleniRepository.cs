using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Zaloha;
using System.Text.Json;

namespace SpravaSkoleni.Data.Repozitare;

/// <summary>
/// Datová vrstva pro práci se souborem skoleni.json.
/// </summary>
public class SkoleniRepository(string aktualniFirma, string slozkaSkoleni = "Skoleni")
{
    private readonly string _aktualniFirma = aktualniFirma;
    private readonly string _slozkaSkoleni = slozkaSkoleni;
    private readonly string _cestaKSouboru = Path.Combine(slozkaSkoleni, "skoleni.json");

    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        WriteIndented = true 
    };

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
            var vsechnaSkoleniZDisku = JsonSerializer.Deserialize<List<Skoleni>>(json) ?? [];

            // --- MULTI-TENANCY FILTR ---
            // Z celého sdíleného souboru vrátíme pouze školení patřící aktuální firmě
            return [.. vsechnaSkoleniZDisku.Where(s => s.Firma.Equals(_aktualniFirma, StringComparison.OrdinalIgnoreCase))];
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

        // 1. Načteme surový stav z disku, abychom nesmazali školení OSTATNÍCH firem
        List<Skoleni> vsechnaSkoleniProZapis = [];
        if (File.Exists(_cestaKSouboru))
        {
            try
            {
                string staryJson = File.ReadAllText(_cestaKSouboru);
                vsechnaSkoleniProZapis = JsonSerializer.Deserialize<List<Skoleni>>(staryJson) ?? [];
            }
            catch { /* Pokud nelze načíst, začneme s prázdným seznamem */ }
        }

        // 2. Odstraníme stará školení POUZE pro aktuální firmu
        vsechnaSkoleniProZapis.RemoveAll(s => s.Firma.Equals(_aktualniFirma, StringComparison.OrdinalIgnoreCase));

        // 3. Všem ukládaným školením explicitně nastavíme/vynutíme naši aktuální firmu a přidáme je do celku
        foreach (var s in seznamSkoleni)
        {
            s.Firma = _aktualniFirma;
        }
        vsechnaSkoleniProZapis.AddRange(seznamSkoleni);

        // 4. Serializujeme kompletní sloučený seznam
        string json = JsonSerializer.Serialize(vsechnaSkoleniProZapis, JsonOptions);

        // Neprůstřelný crash-safe zápis přes dočasný soubor
        string docasnySoubor = _cestaKSouboru + ".tmp";
        File.WriteAllText(docasnySoubor, json);
        File.Move(docasnySoubor, _cestaKSouboru, overwrite: true);

        // Automatická tichá záloha na pozadí (Zůstává beze změny vaše volání externí služby)
        ZalohovaciSluzba.ProvedAutomatickouZalohu(_cestaKSouboru);
    }
}