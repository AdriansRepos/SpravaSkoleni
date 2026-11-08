using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Menu;
using SpravaSkoleni.Enumy;


namespace SpravaSkoleni.Vypisy;

public static class SkoleniMenuVypisy
{
    public static async Task SpustInteraktivniMenuAsync(SpravceSkoleni spravceSkoleni)
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

            var vybranaAkce = SkoleniMenu.ZpracujHlavniVolbu(volba.ToString());

            switch (vybranaAkce)
            {
                case SkoleniMenuHlavniVolba.VytvoritNoveSkoleni:
                    await SkoleniVypisy.VytvoritNoveSkoleniAsync(spravceSkoleni);
                    break;

                case SkoleniMenuHlavniVolba.VybratExistujiciSkoleni:
                    Skoleni? vybraneSkoleni = SkoleniVypisy.VybratSkoleni(spravceSkoleni);
                    if (vybraneSkoleni != null)
                    {
                        await SpravovatDetailSkoleniAsync(spravceSkoleni, vybraneSkoleni);
                    }
                    break;

                case SkoleniMenuHlavniVolba.NávratZpět:
                    return;

                case SkoleniMenuHlavniVolba.NeplatnaVolba:
                    VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");
                    break;
            }
        }
        while (true);
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

            var vybranaDetailAkce = SkoleniMenu.ZpracujDetailVolbu(volbaDetailSkoleni.ToString());
            
            switch (vybranaDetailAkce)
            {
                case SkoleniDetailMenuVolba.PrihlasitStudenta:                                    
                    await SkoleniVypisy.PrihlasitStudentaAsync(spravceSkoleni, vybraneSkoleni);
                    break;

                case SkoleniDetailMenuVolba.OdhlasitStudenta:                                    
                    await SkoleniVypisy.OdhlasitStudentaAsync(spravceSkoleni, vybraneSkoleni);
                    break;

                case SkoleniDetailMenuVolba.PriraditLektora:                                    
                    await SkoleniVypisy.PriraditLektoraAsync(spravceSkoleni, vybraneSkoleni);
                    break;

                case SkoleniDetailMenuVolba.OdebratLektora:                                    
                    await SkoleniVypisy.OdebratLektoraAsync(spravceSkoleni, vybraneSkoleni);
                    break;

                case SkoleniDetailMenuVolba.UzavritSkoleni:                                    
                    await SkoleniVypisy.UzavritSkoleniAsync(spravceSkoleni, vybraneSkoleni);
                    break;

                case SkoleniDetailMenuVolba.NávratZpět:
                    return;

                case SkoleniDetailMenuVolba.NeplatnaVolba:                                         
                    VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");                                         
                    break;
            }
        }
        while (true);
    }
}