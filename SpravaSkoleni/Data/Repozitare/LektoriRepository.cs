using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Zaloha;
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

        public void Ulozit(List<Lektor> lektori)
        {
            // Auto-generování složek (All-in-One pojistka)
            Directory.CreateDirectory(_slozkaLektori);
            
            // Serializace lektorů
            var zaznamyLektoru = new List<ZamestnanecZaznam>();
            foreach (var l in lektori)
                zaznamyLektoru.Add(new ZamestnanecZaznam(l.Jmeno, l.Prijmeni, l.Kod!, l.PracovniPozice, JeLektorem: true));
            
            string jsonLektori = JsonSerializer.Serialize(zaznamyLektoru, JsonOptions);
            string tmpLektori = _cestaLektori + ".tmp";
            File.WriteAllText(tmpLektori, jsonLektori);
            File.Move(tmpLektori, _cestaLektori, overwrite: true);
            ZalohovaciSluzba.ProvedAutomatickouZalohu(_cestaLektori);            
        }
    }
}
