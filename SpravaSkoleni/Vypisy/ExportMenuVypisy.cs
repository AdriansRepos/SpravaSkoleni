using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Menu;
using SpravaSkoleni.Enumy;

namespace SpravaSkoleni.Vypisy;

public static class ExportMenuVypisy
{
    public static async Task SpustInteraktivniMenuAsync(SpravceSkoleni spravceSkoleni)
    {
        char volba;
        do
        {
            VypisyDoKonzole.VypisHlavickuMenu("MENU EXPORTU");
            VypisyDoKonzole.VypisTeloMenu(
                "1. Vyexportovat prezenční listinu školení",
                "n. Návrat do hlavního menu"
            );

            VypisyDoKonzole.VypisVyzvuKZadani();
            volba = char.ToLower(Console.ReadKey().KeyChar);
            Console.WriteLine();

            var vybranaAkce = ExportMenu.ZpracujVolbu(volba.ToString());

            switch (vybranaAkce)
            {
                case ExportMenuVolba.ProvéstExport:
                    Console.WriteLine("\n*** Výběr školení k exportu ***\n");
                    Skoleni? vybraneSkoleniKExportu = SkoleniVypisy.VybratSkoleni(spravceSkoleni);

                    if (vybraneSkoleniKExportu != null)
                    {
                        try
                        {
                            await SpravceExportu.ExportujPrezencniListinuAsync(vybraneSkoleniKExportu);
                            VypisyDoKonzole.VypisUspech("! Prezenční listina byla úspěšně vyexportována do složky Exporty !");
                        }
                        catch (IOException ex)
                        {
                            VypisyDoKonzole.VypisVarovani(ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Žádné školení nebylo vybráno nebo neexistuje.");
                    }
                    
                    VypisyDoKonzole.VypisInformaci("\nPro návrat stiskněte libovolnou klávesu...");
                    Console.ReadKey();
                    break;

                case ExportMenuVolba.NávratZpět:
                    return;

                case ExportMenuVolba.NeplatnaVolba:
                    VypisyDoKonzole.VypisInformaci("Neplatná volba, opakujte zadání:");
                    break;
            }
        }
        while (true);
    }
}