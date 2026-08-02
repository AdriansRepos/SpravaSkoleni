using System.Text.Json;

namespace SpravaSkoleni
{
    public partial class SpravceSkoleni
    {
        private static readonly JsonSerializerOptions SkoleniJsonOptions = new()
        {
            WriteIndented = true
        };

        private const string SouborSkoleniJson = "skoleni.json";

        /// <summary>
        /// Uloží všechna školení do JSON souboru. Ukládají se jen kódy
        /// studentů/lektorů a hodnocení, ne celé objekty.
        /// </summary>
        public void UlozitSkoleniJson()
        {
            List<Skoleni> kUlozeni = [];

            foreach (var skoleni in SeznamSkoleni)
            {
                skoleni.KodyStudentu = skoleni.Studenti.Select(s => s.Kod!).ToList();
                skoleni.KodyLektoru = skoleni.Lektori.Select(l => l.Kod!).ToList();
                skoleni.Hodnoceni = skoleni.HodnoceniStudentu
                    .ToDictionary(kv => kv.Key.Kod!, kv => kv.Value);

                kUlozeni.Add(skoleni);
            }

            string json = JsonSerializer.Serialize(kUlozeni, SkoleniJsonOptions);
            File.WriteAllText(SouborSkoleniJson, json);
        }

        /// <summary>
        /// Načte školení z JSON souboru a dopojí skutečné objekty
        /// studentů/lektorů podle uložených kódů.
        /// </summary>
        public void NacistSkoleniJson()
        {
            if (!File.Exists(SouborSkoleniJson))
                return;

            try
            {
                string json = File.ReadAllText(SouborSkoleniJson);
                var nactena = JsonSerializer.Deserialize<List<Skoleni>>(json, SkoleniJsonOptions) ?? [];

                foreach (var skoleni in nactena)
                {
                    skoleni.Studenti = skoleni.KodyStudentu
                        .Select(kod => NajdiStudentaPodleKodu(kod))
                        .Where(s => s != null)
                        .Select(s => s!)
                        .ToList();

                    skoleni.Lektori = skoleni.KodyLektoru
                        .Select(kod => NajdiLektoraPodleKodu(kod))
                        .Where(l => l != null)
                        .Select(l => l!)
                        .ToList();

                    skoleni.HodnoceniStudentu = skoleni.Hodnoceni
                        .Where(kv => NajdiStudentaPodleKodu(kv.Key) != null)
                        .ToDictionary(kv => NajdiStudentaPodleKodu(kv.Key)!, kv => kv.Value);

                    foreach (var student in skoleni.Studenti)
                        student.Skoleni.Add(skoleni);

                    foreach (var lektor in skoleni.Lektori)
                        lektor.VyucovanaSkoleni.Add(skoleni);
                }

                SeznamSkoleni = nactena;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání skoleni.json: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Jednorázová migrace dat ze starého textového formátu (skoleni.txt)
        /// do nového JSON formátu (skoleni.json). Po úspěšné migraci lze tuto
        /// metodu i staré txt soubory odstranit.
        /// </summary>
        public void MigrujSkoleniZeStarehoFormatu()
        {
            if (File.Exists(SouborSkoleniJson))
            {
                Console.WriteLine("skoleni.json už existuje, migrace se přeskakuje.");
                return;
            }
        
            if (!File.Exists("skoleni.txt"))
            {
                Console.WriteLine("skoleni.txt neexistuje, není co migrovat.");
                return;
            }
        
            NacistSkoleni();       // stará metoda (txt) – naplní SeznamSkoleni
            UlozitSkoleniJson();   // nová metoda – zapíše totéž do JSON
        
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Migrace dokončena: {SeznamSkoleni.Count} školení převedeno do skoleni.json.");
            Console.ResetColor();
        }
    }
}