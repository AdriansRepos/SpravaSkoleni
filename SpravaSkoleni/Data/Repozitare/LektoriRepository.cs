using SpravaSkoleni.Data.Entity;
using System.Text.Json;

namespace SpravaSkoleni.Data.Repozitare
{
    public class LektoriRepository(string slozkaLektori = "Lektori")
    {
        private readonly string _slozkaLektori = slozkaLektori;

        private readonly string _cestaLektori = Path.Combine(slozkaLektori, "lektori.json");

        private static readonly JsonSerializerOptions JsonOptions = new() 
        { 
            WriteIndented = true 
        };

        // Pomocný datový záznam pro čisté ukládání (DTO)
        private record ZamestnanecZaznam(string Jmeno, string Prijmeni, string Kod, string PracovniPozice, bool JeLektorem);

        public (List<Lektor> Lektori) NacistVsechny()
        {
            var lektori = new List<Lektor>();
            
            // 1. Tichá obnova lektorů ze zálohy v případě havárie
            string zalohaLektori = Path.Combine("Zalohy", "lektori.bak.json");
            if (!File.Exists(_cestaLektori) && File.Exists(zalohaLektori))
            {
                try { Directory.CreateDirectory(_slozkaLektori); File.Copy(zalohaLektori, _cestaLektori); } catch {}
            }
                        
            // Načtení a spárování lektorů
            if (File.Exists(_cestaLektori))
            {
                try
                {
                    string json = File.ReadAllText(_cestaLektori);
                    var zaznamy = JsonSerializer.Deserialize<List<ZamestnanecZaznam>>(json) ?? [];
                    foreach (var z in zaznamy)
                        lektori.Add(new Lektor(z.Jmeno, z.Prijmeni, z.Kod, z.PracovniPozice));
                }
                catch {}
            }            
            return (lektori);
        }

    }
}
