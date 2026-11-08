using BarevneVypisyHelper;
using SelectHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Menu;
using TextHelper;
using SpravaSkoleni.Enumy;


namespace SpravaSkoleni.Vypisy;

public static class VyhledavaniMenuVypisy
{
    public static void SpustInteraktivniMenu(SpravceSkoleni spravceSkoleni)
    {
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

            var vybranaAkce = VyhledavaniMenu.ZpracujVolbu(volbaVyhledavani.ToString());
            IEnumerable<Skoleni>? filtrovanySeznam = null;

            switch (vybranaAkce)
            {
                case VyhledavaniMenuVolba.PouzeOtevrena:
                    filtrovanySeznam = spravceSkoleni.GetSkoleniPodleStavu(pouzeOtevrena: true);
                    break;

                case VyhledavaniMenuVolba.PouzeUzavrena:
                    filtrovanySeznam = spravceSkoleni.GetSkoleniPodleStavu(pouzeOtevrena: false);
                    break;

                case VyhledavaniMenuVolba.PodleTextu:
                    Console.Write("Zadejte hledaný text (název, příjmení lektora nebo kód): ");
                    string text = Console.ReadLine()!;
                    filtrovanySeznam = spravceSkoleni.VyhledatSkoleniPodleTextu(text);
                    break;

                case VyhledavaniMenuVolba.PodleRozmeziDatumu:
                    DateTime odData = UpravaVstupu.ZeptejSeAUprav(
                        DateTime.MinValue, "počáteční datum rozmezí (den.měsíc.rok)",
                        v => v.ToShortDateString(), s => DateTime.Parse(s), jeNove: true);

                    DateTime doData = UpravaVstupu.ZeptejSeAUprav(
                        DateTime.MinValue, "koncové datum rozmezí (den.měsíc.rok)",
                        v => v.ToShortDateString(), s => DateTime.Parse(s), jeNove: true);

                    filtrovanySeznam = spravceSkoleni.VyhledatSkoleniVRozmeziDat(odData, doData);
                    break;

                case VyhledavaniMenuVolba.NávratZpět:
                    return; // Ukončí smyčku a vrátí se do hlavního menu

                case VyhledavaniMenuVolba.NeplatnaVolba:
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
        while (true);
    }
}