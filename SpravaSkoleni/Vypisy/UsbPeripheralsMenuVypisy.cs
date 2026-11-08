using BarevneVypisyHelper;

namespace SpravaSkoleni.Vypisy;

public static class UsbPeripheralsMenuVypisy
{
    public static void ZobrazUvodniObrazovku()
    {
        Console.Clear();
        VypisyDoKonzole.VypisHlavickuMenu("SPRÁVA USB, TISKÁRN A PDF");
    }

    public static void ZobrazTeloMenu()
    {
        VypisyDoKonzole.VypisTeloMenu(
            "1. Zdetekovat připojené USB disky",
            "2. Exportovat soubor/zálohu na USB disk",
            "3. Vygenerovat PDF dokument (např. potvrzení)",
            "4. Zobrazit dostupné systémové tiskárny",
            "5. Tisknout na systémovou tiskárnu",
            "6. Tisknout na síťovou tiskárnu (IP / port)",
            "0. Návrat do hlavního menu"
        );
    }

    public static void VypisChybu(string zprava) => VypisyDoKonzole.VypisVarovani(zprava);        // Červená
    public static void VypisVarovaniVstupu(string zprava) => VypisyDoKonzole.VypisInformaci(zprava); // Žlutá (neplatný vstup / opakované zadání)
    public static void VypisUspesnouZpravu(string zprava) => VypisyDoKonzole.VypisUspech(zprava);      // Zelená
    public static void VypisInfo(string zprava) => VypisyDoKonzole.VypisZvyrazneni(zprava);           // Azurová
}