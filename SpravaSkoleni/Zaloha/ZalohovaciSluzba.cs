using BarevneVypisyHelper;

namespace SpravaSkoleni.Zaloha
{
    public static class ZalohovaciSluzba
    {
        private const string SlozkaAutomatickychZaloh = "Zalohy";

        /// <summary>
        /// Provede tichou automatickou zálohu po každém úspěšném uložení dat.
        /// Uživatel je od tohoto procesu zcela odstíněn.
        /// </summary>
        public static void ProvedAutomatickouZalohu(string cestaKSouboru, string aktualniFirma)
        {
            try
            {
                // Pokud složka pro běžné automatické zálohy neexistuje, bezpečně ji založíme
                if (!Directory.Exists(SlozkaAutomatickychZaloh))
                {
                    Directory.CreateDirectory(SlozkaAutomatickychZaloh);
                }

                string nazevSouboru = Path.GetFileNameWithoutExtension(cestaKSouboru); // např. "skoleni"
                // Vytvoříme unikátní název zálohy pro danou firmu, např. "skoleni_SkodaAuto.bak.json"
                string cilovaCesta = Path.Combine(SlozkaAutomatickychZaloh, $"{nazevSouboru}_{aktualniFirma}.bak.json");

                File.Copy(cestaKSouboru, cilovaCesta, overwrite: true);

                // Provede se rychlá, tichá záloha na disku
                File.Copy(cestaKSouboru, cilovaCesta, overwrite: true);
            }
            catch (Exception)
            {
                // Tichá ignorance, aby případný I/O zámek disku neshodil běžící aplikaci
            }
        }
    }
}