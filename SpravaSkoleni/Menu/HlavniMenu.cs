using BarevneVypisyHelper;
using SpravaSkoleni.Logika;
using TextHelper;

namespace SpravaSkoleni.Menu;

public static class HlavniMenu
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
                "k. Ukončit program"
            );
            
            VypisyDoKonzole.VypisVyzvuKZadani();

            volba = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            switch (volba)
            {
                case '1': 
                    await SkoleniMenu.SpustitAsync(spravceSkoleni);
                    break;

                case '2': 
                    await ExportMenu.SpustitAsync(spravceSkoleni);
                    break;

                case '3': 
                    await VyhledavaniMenu.SpustitAsync(spravceSkoleni);
                    break;

                case '4': 
                    await UcastniciMenu.SpustitAsync(spravceSkoleni);
                    break;                
                
                case 'k':                 
                    break;

                default:                         
                    VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");                         
                    break;
            }
        } 
        while (volba != 'k');
    }
}