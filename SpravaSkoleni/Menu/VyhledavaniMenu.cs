using SpravaSkoleni.Enumy;

namespace SpravaSkoleni.Menu;

public class VyhledavaniMenu
{
    public static VyhledavaniMenuVolba ZpracujVolbu(string vstup)
    {
        return vstup.Trim().ToLower() switch
        {
            "1" => VyhledavaniMenuVolba.PouzeOtevrena,
            "2" => VyhledavaniMenuVolba.PouzeUzavrena,
            "3" => VyhledavaniMenuVolba.PodleTextu,
            "4" => VyhledavaniMenuVolba.PodleRozmeziDatumu,
            "n" => VyhledavaniMenuVolba.NávratZpět,
            _ => VyhledavaniMenuVolba.NeplatnaVolba
        };
    }
}