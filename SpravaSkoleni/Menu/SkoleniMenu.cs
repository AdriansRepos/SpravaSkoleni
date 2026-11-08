namespace SpravaSkoleni.Menu;

public enum SkoleniMenuHlavniVolba
{
    NávratZpět,
    VytvoritNoveSkoleni,
    VybratExistujiciSkoleni,
    NeplatnaVolba
}

public enum SkoleniDetailMenuVolba
{
    NávratZpět,
    PrihlasitStudenta,
    OdhlasitStudenta,
    PriraditLektora,
    OdebratLektora,
    UzavritSkoleni,
    NeplatnaVolba
}

public class SkoleniMenu
{
    public static SkoleniMenuHlavniVolba ZpracujHlavniVolbu(string vstup)
    {
        return vstup.Trim().ToLower() switch
        {
            "1" => SkoleniMenuHlavniVolba.VytvoritNoveSkoleni,
            "2" => SkoleniMenuHlavniVolba.VybratExistujiciSkoleni,
            "n" => SkoleniMenuHlavniVolba.NávratZpět,
            _ => SkoleniMenuHlavniVolba.NeplatnaVolba
        };
    }

    public static SkoleniDetailMenuVolba ZpracujDetailVolbu(string vstup)
    {
        return vstup.Trim().ToLower() switch
        {
            "1" => SkoleniDetailMenuVolba.PrihlasitStudenta,
            "2" => SkoleniDetailMenuVolba.OdhlasitStudenta,
            "3" => SkoleniDetailMenuVolba.PriraditLektora,
            "4" => SkoleniDetailMenuVolba.OdebratLektora,
            "5" => SkoleniDetailMenuVolba.UzavritSkoleni,
            "n" => SkoleniDetailMenuVolba.NávratZpět,
            _ => SkoleniDetailMenuVolba.NeplatnaVolba
        };
    }
}