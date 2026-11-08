namespace SpravaSkoleni.Menu;

public enum VyhledavaniMenuVolba
{
    NávratZpět,
    PouzeOtevrena,
    PouzeUzavrena,
    PodleTextu,
    PodleRozmeziDatumu,
    NeplatnaVolba
}

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