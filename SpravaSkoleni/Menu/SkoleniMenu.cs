using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Vypisy;

namespace SpravaSkoleni.Menu;

public static class SkoleniMenu
{
    public static async Task SpustitAsync(SpravceSkoleni spravceSkoleni)
    {
        char volba;
        do
        {
            VypisyDoKonzole.VypisHlavickuMenu("SPRÁVA ŠKOLENÍ");
            VypisyDoKonzole.VypisTeloMenu(
                "1. Vytvořit nové školení",
                "2. Vybrat existující školení (Detail / Úpravy)",
                "n. Návrat do hlavního menu"
            );

            VypisyDoKonzole.VypisVyzvuKZadani();
            volba = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            switch (volba)
            {
                case '1':
                    await SkoleniVypisy.VytvoritNoveSkoleniAsync(spravceSkoleni);
                    break;

                case '2':
                    Skoleni? vybraneSkoleni = SkoleniVypisy.VybratSkoleni(spravceSkoleni);
                    if (vybraneSkoleni != null)
                    {
                        await SpravovatDetailSkoleniAsync(spravceSkoleni, vybraneSkoleni);
                    }
                    break;

                case 'n':
                    break;

                default:
                    VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");
                    break;
            }
        }
        while (volba != 'n');
    }

    private static async Task SpravovatDetailSkoleniAsync(SpravceSkoleni spravceSkoleni, Skoleni vybraneSkoleni)
    {
        SkoleniVypisy.ZobrazDetailSkoleni(vybraneSkoleni);

        if (vybraneSkoleni.Uzavreno)
        {
            VypisyDoKonzole.VypisInformaci("! Školení je uzavřené a nelze s ním provést žádnou operaci." + 
                "\nPro návrat zpět stiskněte libovolnou klávesu !");
            Console.ReadKey();
            return;
        }

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
                "n. Návrat zpět"
            );
            
            VypisyDoKonzole.VypisVyzvuKZadani();
            volbaDetailSkoleni = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();
            
            switch (volbaDetailSkoleni)
            {
                case '1':                                    
                    await SkoleniVypisy.PrihlasitStudentaAsync(spravceSkoleni, vybraneSkoleni);
                    break;

                case '2':                                    
                    await SkoleniVypisy.OdhlasitStudentaAsync(spravceSkoleni, vybraneSkoleni);
                    break;

                case '3':                                    
                    await SkoleniVypisy.PriraditLektoraAsync(spravceSkoleni, vybraneSkoleni);
                    break;

                case '4':                                    
                    await SkoleniVypisy.OdebratLektoraAsync(spravceSkoleni, vybraneSkoleni);
                    break;

                case '5':                                    
                    await SkoleniVypisy.UzavritSkoleniAsync(spravceSkoleni, vybraneSkoleni);
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