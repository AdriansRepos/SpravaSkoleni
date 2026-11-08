namespace SpravaSkoleni.Menu;

public enum ExportMenuVolba
{
    NávratZpět,
    ProvéstExport,
    NeplatnaVolba
}

public class ExportMenu
{
    public static ExportMenuVolba ZpracujVolbu(string vstup)
    {
        return vstup.Trim().ToLower() switch
        {
            "1" => ExportMenuVolba.ProvéstExport,
            "n" => ExportMenuVolba.NávratZpět,
            _ => ExportMenuVolba.NeplatnaVolba
        };
    }
}