using SpravaSkoleni.Enumy;

namespace SpravaSkoleni.Menu;

public class HlavniMenu
{
    public static HlavniMenuVolba ZpracujVolbu(string vstup)
    {
        return vstup.Trim().ToLower() switch
        {
            "1" => HlavniMenuVolba.SpravaSkoleni,
            "2" => HlavniMenuVolba.ExportPrezencniListiny,
            "3" => HlavniMenuVolba.VyhledavaniAFiltrace,
            "4" => HlavniMenuVolba.SpravaUcastniku,
            "5" => HlavniMenuVolba.UsbPeripheralMenu,
            "k" => HlavniMenuVolba.KonecProgramu,
            _ => HlavniMenuVolba.NeplatnaVolba
        };
    }
}