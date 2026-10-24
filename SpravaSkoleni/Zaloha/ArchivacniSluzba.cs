namespace SpravaSkoleni.Zaloha
{
    public class ArchivacniSluzba
    {
        private const string ArchivSlozka = "Archiv";

        /// <summary>
        /// Zkontroluje a provede automatickou roční archivaci souborů na pozadí.
        /// Uživatel je od tohoto procesu zcela odstíněn.
        /// </summary>
        public static void ZkontrolujRocniArchivaci(string souborZamestnanci = "zamestnanci.json", string souborSkoleni = "skoleni.json")
        {
            try
            {
                if (!Directory.Exists(ArchivSlozka))
                {
                    Directory.CreateDirectory(ArchivSlozka);
                }

                int aktualniRok = DateTime.Now.Year;
                // Provedeme archivaci za minulý rok, pokud ještě neproběhla
                int rokKArchivaci = aktualniRok - 1;

                ArchivujSoubor(souborZamestnanci, "zamestnanci", rokKArchivaci);
                ArchivujSoubor(souborSkoleni, "skoleni", rokKArchivaci);

                VycistiStareArchivy(aktualniRok);
            }
            catch (Exception)
            {
                // Tichá ignorance, aby selhání I/O disku nezhroutilo aplikaci při startu
            }
        }

        private static void ArchivujSoubor(string zdrojovySoubor, string nazevBaze, int rok)
        {
            if (!File.Exists(zdrojovySoubor))
                return;

            string cil = Path.Combine(ArchivSlozka, $"{nazevBaze}_{rok}.json");

            // Pokud archiv za daný rok už existuje, nepřepisujeme ho
            if (File.Exists(cil))
                return;

            try
            {
                // KROK 3: Neprůstřelný crash-safe zápis i do archivu přes .tmp
                string docasnySoubor = cil + ".tmp";
                File.Copy(zdrojovySoubor, docasnySoubor, overwrite: true);
                File.Move(docasnySoubor, cil, overwrite: true);
            }
            catch (Exception)
            {
                // Tichá záchrana logu na pozadí
            }
        }

        private static void VycistiStareArchivy(int aktualniRok)
        {
            if (!Directory.Exists(ArchivSlozka))
                return;

            // Hledáme soubory odpovídající masce
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