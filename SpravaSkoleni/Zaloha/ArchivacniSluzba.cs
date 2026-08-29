using BarevneVypisyHelper;

namespace SpravaSkoleni.Zaloha
{
    public class ArchivacniSluzba(string archivSlozka = "Archiv")
    {
        private readonly string archivSlozka = archivSlozka;

        /// <summary>
        /// Zkontroluje a provede roční archivaci souborů.
        /// </summary>
        public void ZkontrolujRocniArchivaci(string souborZamestnanci = "zamestnanci.json", string souborSkoleni = "skoleni.json")
        {
            Directory.CreateDirectory(archivSlozka);

            int aktualniRok = DateTime.Now.Year;
            // Provedeme archivaci za minulý rok, pokud ještě neproběhla
            int rokKArchivaci = aktualniRok - 1;

            ArchivujSoubor(souborZamestnanci, "zamestnanci", rokKArchivaci);
            ArchivujSoubor(souborSkoleni, "skoleni", rokKArchivaci);

            VycistiStareArchivy(aktualniRok);
        }

        private void ArchivujSoubor(string zdrojovySoubor, string nazevBaze, int rok)
        {
            if (!File.Exists(zdrojovySoubor))
                return;

            string cil = Path.Combine(archivSlozka, $"{nazevBaze}_{rok}.json");

            // Pokud archiv za daný rok už existuje, nepřepisujeme ho
            if (File.Exists(cil))
                return;

            try
            {
                File.Copy(zdrojovySoubor, cil, overwrite: false);
                VypisyDoKonzole.VypisUspech($"Data ({nazevBaze}) za rok {rok} byla archivována.");
            }
            catch (Exception ex)
            {
                VypisyDoKonzole.VypisVarovani($"Chyba při archivaci souboru '{nazevBaze}' za rok {rok}: {ex.Message}");
            }
        }

        private void VycistiStareArchivy(int aktualniRok)
        {
            if (!Directory.Exists(archivSlozka))
                return;

            foreach (string soubor in Directory.GetFiles(archivSlozka, "*_*.json"))
            {
                string jmenoSouboru = Path.GetFileNameWithoutExtension(soubor);
                int podtrzitko = jmenoSouboru.LastIndexOf('_');
                if (podtrzitko < 0) 
                    continue;

                if (!int.TryParse(jmenoSouboru[(podtrzitko + 1)..], out int rokSouboru))
                    continue;

                // Zaměstnanecká/mzdová data 30 let, historie školení 5 let
                int hraniceRoku = jmenoSouboru.StartsWith("zamestnanci_") ? 30 : 5;

                if (aktualniRok - rokSouboru > hraniceRoku)
                {
                    try
                    {
                        File.Delete(soubor);
                        VypisyDoKonzole.VypisInformaci($"Starý archivní soubor smazán: {Path.GetFileName(soubor)}");
                    }
                    catch (Exception ex)
                    {
                        VypisyDoKonzole.VypisVarovani($"Nepodařilo se smazat archivní soubor {Path.GetFileName(soubor)}: {ex.Message}");
                    }
                }
            }
        }
    }
}