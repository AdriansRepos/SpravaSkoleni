using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Vypisy;

namespace SpravaSkoleni.Menu;

public static class ExportMenu
{
    public static async Task SpustitAsync(SpravceSkoleni spravceSkoleni)
    {
        Console.WriteLine("\n*** Export prezenční listiny ***\n");
            
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
    }
}