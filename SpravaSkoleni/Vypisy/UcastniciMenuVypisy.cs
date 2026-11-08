using BarevneVypisyHelper;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Menu;

namespace SpravaSkoleni.Vypisy;

public static class UcastniciMenuVypisy
{
    public static void SpustInteraktivniMenu(SpravceSkoleni spravceSkoleni)
    {
        char volbaUcastnici;
        do
        {
            VypisyDoKonzole.VypisHlavickuMenu("MENU ÚČASTNÍKŮ A HISTORIE");
            VypisyDoKonzole.VypisTeloMenu(
                "1. Vypsat všechny studenty a jejich historii",
                "2. Vypsat všechny lektory a jejich historii",
                "n. Návrat do hlavního menu"
            );
            
            VypisyDoKonzole.VypisVyzvuKZadani();
            volbaUcastnici = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            // Převod konzolového znaku na náš čistý enum přes logiku z menu
            var vybranaAkcija = UcastniciMenu.ZpracujVolbu(volbaUcastnici.ToString());

            switch (vybranaAkcija)
            {
                case UcastniciMenuVolba.ZobrazitVsechnyStudenty:
                    StudentiVypisy.VypisVsechnyStudenty(spravceSkoleni);
                    VypisyDoKonzole.VypisInformaci("\nPro návrat do podmenu stiskněte libovolnou klávesu...");
                    Console.ReadKey();
                    break;

                case UcastniciMenuVolba.ZobrazitVsechnyLektory:
                    LektoriVypisy.VypisVsechnyLektory(spravceSkoleni);
                    VypisyDoKonzole.VypisInformaci("\nPro návrat do podmenu stiskněte libovolnou klávesu...");
                    Console.ReadKey();
                    break;

                case UcastniciMenuVolba.NávratZpět:
                    if (volbaUcastnici == 'n') 
                        return; // Ukončí smyčku a vrátí se výš
                    
                    VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");
                    break;
            }
        } 
        while (true);
    }
}
