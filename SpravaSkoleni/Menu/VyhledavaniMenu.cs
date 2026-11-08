using BarevneVypisyHelper;
using SelectHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Vypisy;
using TextHelper;

namespace SpravaSkoleni.Menu;

public static class VyhledavaniMenu
{
    public static async Task SpustitAsync(SpravceSkoleni spravceSkoleni)
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
    }
}