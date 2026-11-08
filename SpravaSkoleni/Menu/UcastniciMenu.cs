using BarevneVypisyHelper;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Vypisy;

namespace SpravaSkoleni.Menu;

public static class UcastniciMenu
{
    public static Task SpustitAsync(SpravceSkoleni spravceSkoleni)
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

            switch (volbaUcastnici)
            {
                case '1':
                    StudentiVypisy.VypisVsechnyStudenty(spravceSkoleni);
                    VypisyDoKonzole.VypisInformaci("\nPro návrat do podmenu stiskněte libovolnou klávesu...");
                    Console.ReadKey();
                    break;

                case '2':
                    LektoriVypisy.VypisVsechnyLektory(spravceSkoleni);
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
        return Task.CompletedTask;
    }
}