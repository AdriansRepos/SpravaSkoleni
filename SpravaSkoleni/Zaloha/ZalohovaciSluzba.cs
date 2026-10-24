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
        public static void ProvedAutomatickouZalohu(string cestaKSouboru)
        {
            try
            {
                // Pokud složka pro běžné automatické zálohy neexistuje, bezpečně ji založíme
                if (!Directory.Exists(SlozkaAutomatickychZaloh))
                {
                    Directory.CreateDirectory(SlozkaAutomatickychZaloh);
                }

                string nazevSouboru = Path.GetFileName(cestaKSouboru);
                // Vytvoříme záložní soubor s příponou .bak.json ve složce Zalohy/
                string cilovaCesta = Path.Combine(SlozkaAutomatickychZaloh, nazevSouboru.Replace(".json", ".bak.json"));

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