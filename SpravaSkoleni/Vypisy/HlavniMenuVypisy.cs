using BarevneVypisyHelper;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Enumy;
using SpravaSkoleni.Menu;
using System.Runtime.Versioning;
using TextHelper;

namespace SpravaSkoleni.Vypisy;

[SupportedOSPlatform("windows")]
public static class HlavniMenuVypisy
{
    public static async Task SpustitAplikaciAsync()
    {
        VypisyDoKonzole.VypisHlavickuMenu("PŘIHLÁŠENÍ DO SYSTÉMU");
    
        string vybranaFirma = UpravaVstupu.ZeptejSeAUprav(
            aktualni: "", 
            popis: "název vaší firmy (Tenant)", 
            formatter: v => v, 
            parser: s => 
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    throw new ArgumentException("Název firmy nesmí být prázdný!");
                }
                return s.Trim();
            }, 
            jeNove: true
        );
    
        SpravceSkoleni spravceSkoleni = new(vybranaFirma);
        await spravceSkoleni.InicializovatAsync();
        await ZobrazVyberMenuAsync(spravceSkoleni);
    }

    public static async Task ZobrazVyberMenuAsync(SpravceSkoleni spravceSkoleni)
    {
        char volba;
        do
        {                
            VypisyDoKonzole.VypisHlavickuMenu("HLAVNÍ MENU");
            VypisyDoKonzole.VypisTeloMenu(
                "1. Správa školení",
                "2. Export prezenční listiny",
                "3. Vyhledávání a filtrace",
                "4. Správa účastníků",   
                "5. Správa USB, tiskáren a PDF",
                "k. Ukončit program"
            );
            
            VypisyDoKonzole.VypisVyzvuKZadani();

            volba = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            var vybranaAkce = HlavniMenu.ZpracujVolbu(volba.ToString());

            switch (vybranaAkce)
            {
                case HlavniMenuVolba.SpravaSkoleni: 
                    await SkoleniMenuVypisy.SpustInteraktivniMenuAsync(spravceSkoleni);
                    break;

                case HlavniMenuVolba.ExportPrezencniListiny: 
                    await ExportMenuVypisy.SpustInteraktivniMenuAsync(spravceSkoleni);
                    break;

                case HlavniMenuVolba.VyhledavaniAFiltrace: 
                    VyhledavaniMenuVypisy.SpustInteraktivniMenu(spravceSkoleni);
                    break;

                case HlavniMenuVolba.SpravaUcastniku: 
                    UcastniciMenuVypisy.SpustInteraktivniMenu(spravceSkoleni);
                    break;   
                    
                case HlavniMenuVolba.UsbPeripheralMenu:
                    UsbPeripheralsMenuVypisy.SpustitInteraktivniMenu();
                    break;

                case HlavniMenuVolba.KonecProgramu:                 
                    break;

                case HlavniMenuVolba.NeplatnaVolba:                         
                    VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");                         
                    break;
            }
        } 
        while (volba != 'k');
    }
}