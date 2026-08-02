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
    }
}