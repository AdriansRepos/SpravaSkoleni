
namespace SpravaSkoleni.Zaloha
{
    public static class ZalohovaciSluzba
    {
        private const string SlozkaAutomatickychZaloh = "Zalohy";

        /// <summary>
        /// Provede tichou automatickou zálohu na pozadí (Multithreaded Fire-and-Forget).
        /// Hlavní kód na dokončení této operace nečeká a pokračuje dál.
        /// </summary>
        public static void ProvedAutomatickouZalohu(string cestaKSouboru, string aktualniFirma)
        {
            // Obalení celé operace do Task.Run() ji okamžitě odepne z hlavního vlákna 
            // a zpracuje asynchronně na pozadí přes Thread Pool.
            Task.Run(async () =>
            {
                try
                {
                    if (!Directory.Exists(SlozkaAutomatickychZaloh))
                    {
                        Directory.CreateDirectory(SlozkaAutomatickychZaloh);
                    }

                    string nazevSouboru = Path.GetFileNameWithoutExtension(cestaKSouboru); 
                    string cilovaCesta = Path.Combine(SlozkaAutomatickychZaloh, $"{nazevSouboru}_{aktualniFirma}.bak.json");

                    // Použijeme asynchronní proudy (Streams) pro neblokující kopírování na disku
                    using FileStream zdrojovyStream = File.OpenRead(cestaKSouboru);
                    using FileStream cilovyStream = File.Create(cilovaCesta);
                    await zdrojovyStream.CopyToAsync(cilovyStream);
                }
                catch (Exception)
                {
                    // Tichá ignorance, aby případný I/O zámek disku neshodil běžící aplikaci
                }
            });
        }

        /// <summary>
        /// Provede globální automatickou zálohu na pozadí (Multithreaded Fire-and-Forget).
        /// </summary>
        public static void ProvedAutomatickouZalohu(string cestaKSouboru)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (!Directory.Exists(SlozkaAutomatickychZaloh))
                    {
                        Directory.CreateDirectory(SlozkaAutomatickychZaloh);
                    }
            
                    string nazevSouboru = Path.GetFileName(cestaKSouboru); 
                    string cilovaCesta = Path.Combine(SlozkaAutomatickychZaloh, nazevSouboru.Replace(".json", ".bak.json"));

                    using FileStream zdrojovyStream = File.OpenRead(cestaKSouboru);
                    using FileStream cilovyStream = File.Create(cilovaCesta);
                    await zdrojovyStream.CopyToAsync(cilovyStream);
                }
                catch (Exception)
                {
                    // Tichá ignorance
                }
            });
        }
    }
}
