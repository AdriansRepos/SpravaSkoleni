using BarevneVypisyHelper;
using SpravaSkoleni.Menu;
using SpravaSkoleni.Enumy;

namespace SpravaSkoleni.Vypisy;

public static class UsbPeripheralsMenuVypisy
{
    public static void SpustitInteraktivniMenu()
    {
        bool pokracovat = true;
        while (pokracovat)
        {
            Console.Clear();
            VypisyDoKonzole.VypisHlavickuMenu("SPRÁVA USB, TISKÁRN A PDF");
            VypisyDoKonzole.VypisTeloMenu(
                "1. Zdetekovat připojené USB disky",
                "2. Exportovat soubor/zálohu na USB disk",
                "3. Vygenerovat PDF dokument (např. potvrzení)",
                "4. Zobrazit dostupné systémové tiskárny",
                "5. Tisknout na systémovou tiskárnu",
                "6. Tisknout na síťovou tiskárnu (IP / port)",
                "0. Návrat do hlavního menu"
            );

            Console.WriteLine();
            VypisyDoKonzole.VypisVyzvuKZadani("Zvolte možnost: ");
            string? vstup = Console.ReadLine();

            var volba = UsbPeripheralsMenu.ZpracujVolbu(vstup ?? "");

            switch (volba)
            {
                case UsbMenuVolba.DostupneDisky:
                    ZpracujDostupneDiskyUI();
                    break;
                case UsbMenuVolba.ExportNaUsb:
                    ZpracujExportUI();
                    break;
                case UsbMenuVolba.GenerovatPdf:
                    ZpracujPdfUI();
                    break;
                case UsbMenuVolba.SystemoveTiskarny:
                    ZpracujSystemoveTiskarnyUI();
                    break;
                case UsbMenuVolba.TiskNaSystemovou:
                    ZpracujTiskSystemUI();
                    break;
                case UsbMenuVolba.TiskNaSitovou:
                    ZpracujTiskSitUI();
                    break;
                case UsbMenuVolba.NavratZpet:
                    pokracovat = false;
                    break;
                case UsbMenuVolba.NeplatnaVolba:
                default:
                    VypisyDoKonzole.VypisInformaci("Neplatná volba. Zadejte prosím číslo v rozmezí 0 až 6.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static void ZpracujDostupneDiskyUI()
    {
        Console.Clear();
        VypisyDoKonzole.VypisZvyrazneni("--- Připojené USB disky ---");
        var disky = UsbPeripheralsMenu.ZiskejDisky();

        if (disky.Count == 0)
        {
            VypisyDoKonzole.VypisVarovani("Žádná vyměnitelná USB zařízení nebyla nalezena.");
        }
        else
        {
            foreach (var d in disky)
            {
                VypisyDoKonzole.VypisZvyrazneni($"-> Disk: {d.DriveLetter} | Jmenovka: {d.VolumeLabel} | Volno: {d.FreeSpaceGb} GB z {d.TotalSizeGb} GB");
            }
        }

        Console.WriteLine("\nStiskněte libovolnou klávesu pro návrat...");
        Console.ReadKey();
    }

    private static void ZpracujExportUI()
    {
        Console.Clear();
        VypisyDoKonzole.VypisZvyrazneni("--- Export zálohy na USB ---");

        VypisyDoKonzole.VypisVyzvuKZadani("Zadejte písmeno disku (např. E:\\): ");
        string? drive = Console.ReadLine();

        VypisyDoKonzole.VypisVyzvuKZadani("Zadejte název souboru (např. zaloha_skoleni.txt): ");
        string? fileName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(drive) || string.IsNullOrWhiteSpace(fileName))
        {
            VypisyDoKonzole.VypisInformaci("Chyba: Neplatné nebo prázdné zadané hodnoty. Opakujte akci.");
        }
        else
        {
            bool uspech = UsbPeripheralsMenu.ProvedExport(drive, fileName);

            if (uspech)
                VypisyDoKonzole.VypisUspech("Soubor byl úspěšně zapsán na USB disk.");
            else
                VypisyDoKonzole.VypisVarovani("Zápis na USB selhal. Zkontrolujte, zda je disk připraven.");
        }

        Console.WriteLine("\nStiskněte libovolnou klávesu...");
        Console.ReadKey();
    }

    private static void ZpracujPdfUI()
    {
        Console.Clear();
        VypisyDoKonzole.VypisZvyrazneni("--- Generování PDF dokumentu ---");

        VypisyDoKonzole.VypisVyzvuKZadani("Cílová složka (např. C:\\Temp nebo cesta k USB): ");
        string? dir = Console.ReadLine();

        VypisyDoKonzole.VypisVyzvuKZadani("Název souboru (např. Potvrzeni): ");
        string? file = Console.ReadLine();

        bool uspech = UsbPeripheralsMenu.ProvedPdf(dir ?? "", file ?? "Dokument");

        if (uspech)
            VypisyDoKonzole.VypisUspech("PDF dokument byl úspěšně vygenerován.");
        else
            VypisyDoKonzole.VypisVarovani("Generování PDF selhalo.");

        Console.WriteLine("\nStiskněte libovolnou klávesu...");
        Console.ReadKey();
    }

    private static void ZpracujSystemoveTiskarnyUI()
    {
        Console.Clear();
        VypisyDoKonzole.VypisZvyrazneni("--- Dostupné systémové tiskárny ---");
        var tiskarny = UsbPeripheralsMenu.ZiskejSystemoveTiskarny();

        if (tiskarny.Count == 0)
        {
            VypisyDoKonzole.VypisVarovani("V systému nebyly nalezeny žádné tiskárny.");
        }
        else
        {
            foreach (var t in tiskarny)
            {
                string vychozi = t.IsDefault ? " (VÝCHOZÍ)" : "";
                VypisyDoKonzole.VypisZvyrazneni($"-> {t.Name}{vychozi}");
            }
        }

        Console.WriteLine("\nStiskněte libovolnou klávesu...");
        Console.ReadKey();
    }

    private static void ZpracujTiskSystemUI()
    {
        Console.Clear();
        VypisyDoKonzole.VypisZvyrazneni("--- Tisk na systémovou tiskárnu ---");

        VypisyDoKonzole.VypisVyzvuKZadani("Zadejte přesný název tiskárny: ");
        string? printerName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(printerName))
        {
            VypisyDoKonzole.VypisInformaci("Název tiskárny nebyl zadaný.");
            Console.ReadKey();
            return;
        }

        bool uspech = UsbPeripheralsMenu.TiskSystem(printerName);

        if (uspech)
            VypisyDoKonzole.VypisUspech("Úloha byla odeslána do tiskové fronty.");
        else
            VypisyDoKonzole.VypisVarovani("Tisk se nezdařil, tiskárna není platná nebo dostupná.");

        Console.ReadKey();
    }

    private static void ZpracujTiskSitUI()
    {
        Console.Clear();
        VypisyDoKonzole.VypisZvyrazneni("--- Tisk na síťovou tiskárnu (RAW / Port 9100) ---");

        VypisyDoKonzole.VypisVyzvuKZadani("IP adresa tiskárny (např. 192.168.1.50): ");
        string? ip = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(ip))
        {
            VypisyDoKonzole.VypisInformaci("IP adresa nebyla zadaná.");
            Console.ReadKey();
            return;
        }

        bool uspech = UsbPeripheralsMenu.TiskSit(ip);

        if (uspech)
            VypisyDoKonzole.VypisUspech("Data byla úspěšně odeslána na síťovou tiskárnu.");
        else
            VypisyDoKonzole.VypisVarovani("Připojení k síťové tiskárně vypršelo nebo selhalo.");

        Console.ReadKey();
    }
}