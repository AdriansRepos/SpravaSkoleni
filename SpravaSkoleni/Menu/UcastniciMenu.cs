using SpravaSkoleni.Enumy;

namespace SpravaSkoleni.Menu;

public class UcastniciMenu
{
    public static UcastniciMenuVolba ZpracujVolbu(string vstup)
    {
        return vstup.Trim().ToLower() switch
        {
            "1" => UcastniciMenuVolba.ZobrazitVsechnyStudenty,
            "2" => UcastniciMenuVolba.ZobrazitVsechnyLektory,
            _ => UcastniciMenuVolba.NávratZpět
        };
    }
}