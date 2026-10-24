using BarevneVypisyHelper;
using SelectHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Vypisy;
using SpravaSkoleni.Zaloha;
using TextHelper;

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
                "3. Export prezenční listiny",
                "4. Vyhledávání a filtrace",
                "5. Správa účastníků",                
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
                    Console.WriteLine("\n*** Export prezenční listiny ***\n");
                        
                    Skoleni? vybraneSkoleniKExportu = SkoleniVypisy.VybratSkoleni(spravceSkoleni); 
                    
                    if (vybraneSkoleniKExportu != null)
                    {                        
                        if (SpravceExportu.ExportujPrezencniListinu(vybraneSkoleniKExportu))
                        {
                            VypisyDoKonzole.VypisUspech("! Prezenční listina byla úspěšně vyexportována do složky Exporty !");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Žádné školení nebylo vybráno nebo neexistuje.");
                    } 
                    break;

                case '4': 
                    char volbaVyhledavani;
                    do
                    {
                        VypisyDoKonzole.VypisHlavickuMenu("MENU VYHLEDÁVÁNÍ A FILTRACE");
                        VypisyDoKonzole.VypisTeloMenu(
                            "1. Zobrazit pouze OTEVŘENÁ školení",
                            "2. Zobrazit pouze UZAVŘENÁ školení",
                            "3. Vyhledat podle textu (název / příjmení či kód lektora)",
                            "4. Vyhledat v rozmezí datumů konání",
                            "n. Návrat do hlavního menu"
                        );
                        
                        VypisyDoKonzole.VypisVyzvuKZadani();

                        volbaVyhledavani = char.ToLower(Console.ReadKey().KeyChar);
                        Console.WriteLine();

                        IEnumerable<Skoleni>? filtrovanySeznam = null;

                        switch (volbaVyhledavani)
                        {
                            case '1':
                                filtrovanySeznam = spravceSkoleni.GetSkoleniPodleStavu(pouzeOtevrena: true);
                                break;

                            case '2':
                                filtrovanySeznam = spravceSkoleni.GetSkoleniPodleStavu(pouzeOtevrena: false);
                                break;

                            case '3':
                                Console.Write("Zadejte hledaný text (název, příjmení lektora nebo kód): ");
                                string text = Console.ReadLine()!;
                                filtrovanySeznam = spravceSkoleni.VyhledatSkoleniPodleTextu(text);
                                break;

                            case '4':
                                DateTime odData = UpravaVstupu.ZeptejSeAUprav(
                                    DateTime.MinValue, "počáteční datum rozmezí (den.měsíc.rok)",
                                    v => v.ToShortDateString(), s => DateTime.Parse(s), jeNove: true);

                                DateTime doData = UpravaVstupu.ZeptejSeAUprav(
                                    DateTime.MinValue, "koncové datum rozmezí (den.měsíc.rok)",
                                    v => v.ToShortDateString(), s => DateTime.Parse(s), jeNove: true);

                                filtrovanySeznam = spravceSkoleni.VyhledatSkoleniVRozmeziDat(odData, doData);
                                break;

                            case 'n':
                                break;

                            default:
                                VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");
                                break;
                        }

                        // Pokud uživatel vybral filtr (1-4) a seznam existuje, necháme ho rovnou vybrat položku
                        if (filtrovanySeznam != null)
                        {
                            Skoleni? vybraneFiltrovane = SelectHelp.VybratPolozku(
                                filtrovanySeznam.ToList(),
                                s => $"{s.Nazev} - {(s.Uzavreno ? "uzavřené" : "otevřené")} ({s.PocatecniDatum.ToShortDateString()})",
                                "školení");

                            if (vybraneFiltrovane != null)
                            {
                                SkoleniVypisy.ZobrazDetailSkoleni(vybraneFiltrovane);
                                VypisyDoKonzole.VypisInformaci("\nPro návrat zpět stiskněte libovolnou klávesu...");
                                Console.ReadKey();
                            }
                        }
                    }
                    while (volbaVyhledavani != 'n');
                    break;

                case '5': 
                    char volbaUcastnici;
                    do
                    {
                        VypisyDoKonzole.VypisHlavickuMenu("MENU ÚČASTNÍKŮ A HISTORIE");
                        VypisyDoKonzole.VypisTeloMenu(
                            "1. Vypsat všechny studenty a jejich historii",
                            "2. Vyplessat všechny lektory a jejich historii",
                            "n. Návrat do hlavního menu"
                        );
                        
                        VypisyDoKonzole.VypisVyzvuKZadani();

                        volbaUcastnici = char.ToLower(Console.ReadKey().KeyChar);
                        Console.WriteLine();

                        switch (volbaUcastnici)
                        {
                            case '1':
                                ZamestnanciVypisy.VypisVsechnyStudenty(spravceSkoleni);
                                VypisyDoKonzole.VypisInformaci("\nPro návrat do podmenu stiskněte libovolnou klávesu...");
                                Console.ReadKey();
                                break;

                            case '2':
                                ZamestnanciVypisy.VypisVsechnyLektory(spravceSkoleni);
                                VypisyDoKonzole.VypisInformaci("\nPro návrat do podmenu stiskněte libovolnou klávesu...");
                                Console.ReadKey();
                                break;

                            case 'n':
                                break;

                            default:
                                VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");
                                break;
                        }
                    } 
                    while (volbaUcastnici != 'n');
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
