
namespace SpravaSkoleni.Zaloha
{
    public class ArchivacniSluzba
    {
        private const string ArchivSlozka = "Archiv";

        /// <summary>
        /// Zkontroluje a provede automatickou roční archivaci souborů na pozadí asynchronně a paralelně.
        /// </summary>
        // Změna: Metoda je nyní async Task
        public static async Task ZkontrolujRocniArchivaciAsync(
            string souborSkoleni = "Skoleni/skoleni.json", 
            string souborStudenti = "Studenti/studenti.json", 
            string souborLektori = "Lektori/lektori.json")
        {
            try
            {
                if (!Directory.Exists(ArchivSlozka))
                {
                    Directory.CreateDirectory(ArchivSlozka);
                }
        
                int aktualniRok = DateTime.Now.Year;
                int rokKArchivaci = aktualniRok - 1;
        
                // --- MULTITHREADING / PARALELNÍ ZPRACOVÁNÍ ---
                // Spustíme archivaci všech tří souborů současně na pozadí a počkáme, až doběhnou všechny najednou
                await Task.WhenAll(
                    ArchivujSouborAsync(souborSkoleni, "skoleni", rokKArchivaci),
                    ArchivujSouborAsync(souborStudenti, "studenti", rokKArchivaci),
                    ArchivujSouborAsync(souborLektori, "lektori", rokKArchivaci)
                );
        
                VycistiStareArchivy(aktualniRok);
            }
            catch (Exception)
            {
                // Tichá ignorance, aby selhání I/O disku nezhroutilo aplikaci při startu
            }
        }

        // Změna: Pomocná metoda upravena na neblokující async Task
        private static async Task ArchivujSouborAsync(string zdrojovySoubor, string nazevBaze, int rok)
        {
            if (!File.Exists(zdrojovySoubor))
                return;

            string cil = Path.Combine(ArchivSlozka, $"{nazevBaze}_{rok}.json");

            if (File.Exists(cil))
                return;

            try
            {
                string docasnySoubor = cil + ".tmp";
                
                // .NET sice nemá přímé File.CopyAsync, ale čistě asynchronně se to dělá přes proudy (Streams):
                using (FileStream zdrojovyStream = File.OpenRead(zdrojovySoubor))
                using (FileStream cilovyStream = File.Create(docasnySoubor))
                {
                    // Uvolní vlákno po dobu fyzického asynchronního kopírování souboru na disku
                    await zdrojovyStream.CopyToAsync(cilovyStream);
                }

                File.Move(docasnySoubor, cil, overwrite: true);
            }
            catch (Exception)
            {
                // Tichá záchrana logu na pozadí
            }
        }

        // Metoda pracuje pouze se jmény souborů v paměti, I/O operace Delete je blesková, 
        // proto může z hlediska režie zůstat synchronní.
        private static void VycistiStareArchivy(int aktualniRok)
        {
            if (!Directory.Exists(ArchivSlozka))
                return;

            foreach (string soubor in Directory.GetFiles(ArchivSlozka, "*_*.json"))
            {
                string jmenoSouboru = Path.GetFileNameWithoutExtension(soubor);
                int podtrzitko = jmenoSouboru.LastIndexOf('_');
                if (podtrzitko < 0)
                    continue;

                if (!int.TryParse(jmenoSouboru[(podtrzitko + 1)..], out int rokSouboru))
                    continue;

                if (aktualniRok - rokSouboru > 5)
                {
                    try
                    {
                        File.Delete(soubor);
                    }
                    catch (Exception)
                    {
                        // Defenzivní skip, pokud operační systém soubor zrovna zablokoval
                    }
                }
            }
        }
    }
}
