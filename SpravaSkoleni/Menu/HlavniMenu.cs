using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Vypisy;
using SpravaSkoleni.Zaloha;

namespace SpravaSkoleni.Menu
{
    public static class HlavniMenu
{
    public static void ZobrazVyberMenu(SpravceSkoleni spravceSkoleni)
    {
        char volba;
        do
        {                
            VypisyDoKonzole.VypisHlavickuMenu("MENU");
            VypisyDoKonzole.VypisTeloMenu(
                "1. Nové školení",
                "2. Správa školení",
                "3. čeká na implementaci",
                "4. čeká na implementaci",
                "5. čeká na implementaci",
                "8. Ruční záloha",
                "9. Výpis Ladící info",
                "k. Ukončit program"
            );
            
            VypisyDoKonzole.VypisVyzvuKZadani();

            volba = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            switch (volba)
            {
                case '1': 
                    // UI řeší zadání od uživatele a předá hotová data správci
                    SkoleniVypisy.VytvoritNoveSkoleni(spravceSkoleni); 
                    break;

                case '2':
                    // UI vykreslí výběr a vrátí zvolené školení
                    Skoleni? vybraneSkoleni = SkoleniVypisy.VybratSkoleni(spravceSkoleni);

                    if (vybraneSkoleni != null)
                    {
                        SkoleniVypisy.ZobrazDetailSkoleni(vybraneSkoleni);

                        if (vybraneSkoleni.Uzavreno)
                        {
                            VypisyDoKonzole.VypisInformaci("! Školení je uzavřené a nelze s ním provést žádnou operaci." + 
                                "\nPro návrat zpět stiskněte libovolnou klávesu !");
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            char volbaDetailSkoleni;
                            do
                            {                                    
                                VypisyDoKonzole.VypisHlavickuMenu("MENU ÚPRAVY ŠKOLENÍ");
                                VypisyDoKonzole.VypisTeloMenu(
                                    "1. Přihlásit studenta",
                                    "2. Odhlásit studenta",
                                    "3. Přiřadit lektora",
                                    "4. Odebrat lektora",
                                    "5. Uzavřít školení",                         
                                    "n. Návrat do hlavního menu"
                                );
                                
                                VypisyDoKonzole.VypisVyzvuKZadani();

                                volbaDetailSkoleni = char.ToLower(Console.ReadKey().KeyChar);
                                Console.WriteLine();
                                
                                switch (volbaDetailSkoleni)
                                {
                                    case '1':                                    
                                        SkoleniVypisy.PrihlasitStudenta(spravceSkoleni, vybraneSkoleni);
                                        break;

                                    case '2':                                    
                                        SkoleniVypisy.OdhlasitStudenta(spravceSkoleni, vybraneSkoleni);
                                        break;

                                    case '3':                                    
                                        SkoleniVypisy.PriraditLektora(spravceSkoleni, vybraneSkoleni);
                                        break;

                                    case '4':                                    
                                        SkoleniVypisy.OdebratLektora(spravceSkoleni, vybraneSkoleni);
                                        break;

                                    case '5':                                    
                                        SkoleniVypisy.UzavritSkoleni(spravceSkoleni, vybraneSkoleni);
                                        break;

                                    case 'n':
                                        break;

                                    default:                                         
                                        VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");                                         
                                        break;
                                }
                            }
                            while (volbaDetailSkoleni != 'n');
                        }                                       
                    }                
                    break;

                case '3': 
                    // čeká na implementaci 
                    break;

                case '4': 
                    // čeká na implementaci 
                    break;

                case '5': 
                    // čeká na implementaci 
                    break;

                case '8':
                    ZalohovaciSluzba.ProvedRucniZalohu();
                    break;

                case '9':                     
                    spravceSkoleni.VypisLadiciInfo();
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
}
