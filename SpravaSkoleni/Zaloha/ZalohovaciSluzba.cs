using BarevneVypisyHelper;

namespace SpravaSkoleni.Zaloha
{
    public class ZalohovaciSluzba
    {
        private static string NouzovaZalohaSlozka =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SpravaSkoleni", "Zaloha");

        /// <summary>
        /// Provede ruční zálohu celé datové složky.
        /// </summary>
        public static void ProvedRucniZalohu(string zdrojovaSlozka = "Data", string? externiCesta = null)
        {
            if (!Directory.Exists(zdrojovaSlozka))
            {
                VypisyDoKonzole.VypisInformaci("Datová složka neexistuje, není co zálohovat.");
                return;
            }

            ZalohujDoSlozky(zdrojovaSlozka, NouzovaZalohaSlozka, "nouzové zálohy (LOCALAPPDATA)");

            if (!string.IsNullOrWhiteSpace(externiCesta))
            {
                ZalohujDoSlozky(zdrojovaSlozka, externiCesta, "externí zálohy");
            }
            else
            {
                VypisyDoKonzole.VypisInformaci("Externí záložní cesta není nastavena.");
            }
        }

        private static void ZalohujDoSlozky(string zdroj, string cil, string popis)
        {
            try
            {
                Directory.CreateDirectory(cil);
                KopirovatSlozkuRekurzivne(zdroj, cil);
                VypisyDoKonzole.VypisUspech($"Vytvořena záložní kopie ({popis}) v: {cil}");
            }
            catch (Exception ex)
            {
                VypisyDoKonzole.VypisVarovani($"Chyba při vytváření {popis}: {ex.Message}");
            }
        }

        private static void KopirovatSlozkuRekurzivne(string zdroj, string cil)
        {
            Directory.CreateDirectory(cil);

            foreach (string soubor in Directory.GetFiles(zdroj))
            {
                string cilovySoubor = Path.Combine(cil, Path.GetFileName(soubor));
                File.Copy(soubor, cilovySoubor, overwrite: true);
            }

            foreach (string podslozka in Directory.GetDirectories(zdroj))
            {
                string cilovaPodslozka = Path.Combine(cil, Path.GetFileName(podslozka));
                KopirovatSlozkuRekurzivne(podslozka, cilovaPodslozka);
            }
        }
    }
}