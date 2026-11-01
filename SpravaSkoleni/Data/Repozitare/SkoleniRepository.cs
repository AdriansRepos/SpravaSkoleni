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

    // Změna: Návratový typ je Task<List<Skoleni>> a přidán modifikátor async
    public async Task<List<Skoleni>> NacistAsync()
    {
        // Pokud chybí hlavní soubor, zkusíme tichou obnovu ze zálohy
        string cestaKZaloze = Path.Combine("Zalohy", $"skoleni_{_aktualniFirma}.bak.json");
        if (!File.Exists(_cestaKSouboru) && File.Exists(cestaKZaloze))
        {
            try
            {
                Directory.CreateDirectory(_slozkaSkoleni);
                File.Copy(cestaKZaloze, _cestaKSouboru);
            }
            catch { /* Defenzivní skip */ }
        }

        if (!File.Exists(_cestaKSouboru)) 
            return [];

        try
        {
            // Asynchronní čtení z disku uvolní vlákno po dobu I/O operace
            string json = await File.ReadAllTextAsync(_cestaKSouboru);
            var vsechnaSkoleniZDisku = JsonSerializer.Deserialize<List<Skoleni>>(json) ?? [];

            // --- MULTI-TENANCY FILTR ---
            return [.. vsechnaSkoleniZDisku.Where(s => s.Firma.Equals(_aktualniFirma, StringComparison.OrdinalIgnoreCase))];
        }
        catch
        {
            return [];
        }
    }

    // Změna: Metoda vrací Task (odpovídá asynchronnímu void) a je označená jako async
    public async Task UlozitAsync(List<Skoleni> seznamSkoleni)
    {
        // Auto-generování složky na disku
        Directory.CreateDirectory(_slozkaSkoleni);

        // 1. Načteme surový stav z disku asynchronně, abychom nesmazali školení OSTATNÍCH firem
        List<Skoleni> vsechnaSkoleniProZapis = [];
        if (File.Exists(_cestaKSouboru))
        {
            try
            {
                string staryJson = await File.ReadAllTextAsync(_cestaKSouboru);
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

        // Neprůstřelný crash-safe zápis přes dočasný soubor (plně asynchronně)
        string docasnySoubor = _cestaKSouboru + ".tmp";
        await File.WriteAllTextAsync(docasnySoubor, json);
        File.Move(docasnySoubor, _cestaKSouboru, overwrite: true);

        // Automatická tichá záloha na pozadí (Zatím necháváme synchronní volání)
        ZalohovaciSluzba.ProvedAutomatickouZalohu(_cestaKSouboru, _aktualniFirma);
    }
}
