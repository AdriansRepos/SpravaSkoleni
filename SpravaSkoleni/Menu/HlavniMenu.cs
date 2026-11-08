namespace SpravaSkoleni.Menu;

public enum HlavniMenuVolba
{
    KonecProgramu,
    SpravaSkoleni,
    ExportPrezencniListiny,
    VyhledavaniAFiltrace,
    SpravaUcastniku,
    NeplatnaVolba
}

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
            "k" => HlavniMenuVolba.KonecProgramu,
            _ => HlavniMenuVolba.NeplatnaVolba
        };
    }
}