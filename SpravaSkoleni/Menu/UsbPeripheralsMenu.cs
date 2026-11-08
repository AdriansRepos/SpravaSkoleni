using BarevneVypisyHelper;
using SpravaSkoleni.Vypisy;
using System.Runtime.Versioning;
using UsbPeripheralsLib.Documents;
using UsbPeripheralsLib.Printing;
using UsbPeripheralsLib.Storage;

namespace SpravaSkoleni.Menu;

[SupportedOSPlatform("windows")]
public static class UsbPeripheralsMenu
{
    private static readonly UsbManager _usbManager = new();
    private static readonly PdfDocumentGenerator _pdfGenerator = new();
    private static readonly SystemPrinterManager _systemPrinterManager = new();
    private static readonly NetworkPrinterManager _networkPrinterManager = new();

    public static void SpustitMenuAsync()
    {
        bool pokracovat = true;
        while (pokracovat)
        {
            UsbPeripheralsMenuVypisy.ZobrazUvodniObrazovku();
            UsbPeripheralsMenuVypisy.ZobrazTeloMenu();
            
            Console.WriteLine();
            VypisyDoKonzole.VypisVyzvuKZadani("Zvolte možnost: ");

            var volba = Console.ReadLine();

            switch (volba)
            {
                case "1":
                    ZpracujDostupneDisky();
                    break;
                case "2":
                    ZpracujExportNaUsb();
                    break;
                case "3":
                    ZpracujGenerovaniPdf();
                    break;
                case "4":
                    ZpracujSystemoveTiskarny();
                    break;
                case "5":
                    ZpracujTiskNaSystemovou();
                    break;
                case "6":
                    ZpracujTiskNaSitovou();
                    break;
                case "0":
                    pokracovat = false;
                    break;
                default:
                    UsbPeripheralsMenuVypisy.VypisVarovaniVstupu("Neplatná volba. Zadejte prosím číslo v rozmezí 0 až 6.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static void ZpracujDostupneDisky()
    {
        Console.Clear();
        UsbPeripheralsMenuVypisy.VypisInfo("--- Připojené USB disky ---");
        var disky = _usbManager.GetConnectedUsbDrives().ToList();

        if (disky.Count == 0)
        {
            UsbPeripheralsMenuVypisy.VypisChybu("Žádná vyměnitelná USB zařízení nebyla nalezena.");
        }
        else
        {
            foreach (var d in disky)
            {
                UsbPeripheralsMenuVypisy.VypisInfo($"-> Disk: {d.DriveLetter} | Jmenovka: {d.VolumeLabel} | Volno: {d.FreeSpaceGb} GB z {d.TotalSizeGb} GB");
            }
        }

        Console.WriteLine("\nStiskněte libovolnou klávesu pro návrat...");
        Console.ReadKey();
    }

    private static void ZpracujExportNaUsb()
    {
        Console.Clear();
        UsbPeripheralsMenuVypisy.VypisInfo("--- Export zálohy na USB ---");
        
        VypisyDoKonzole.VypisVyzvuKZadani("Zadejte písmeno disku (např. E:\\): ");
        string? drive = Console.ReadLine();

        VypisyDoKonzole.VypisVyzvuKZadani("Zadejte název souboru (např. zaloha_skoleni.txt): ");
        string? fileName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(drive) || string.IsNullOrWhiteSpace(fileName))
        {
            UsbPeripheralsMenuVypisy.VypisVarovaniVstupu("Chyba: Neplatné nebo prázdné zadané hodnoty. Opakujte akci.");
        }
        else
        {
            string obsah = "Automatická záloha dat ze systému SpravaSkoleni vytvořena dne " + DateTime.Now;
            bool uspech = _usbManager.ExportFileToUsb(drive, fileName, obsah);

            if (uspech)
                UsbPeripheralsMenuVypisy.VypisUspesnouZpravu("Soubor byl úspěšně zapsán na USB disk.");
            else
                UsbPeripheralsMenuVypisy.VypisChybu("Zápis na USB selhal. Zkontrolujte, zda je disk připraven.");
        }

        Console.WriteLine("\nStiskněte libovolnou klávesu...");
        Console.ReadKey();
    }

    private static void ZpracujGenerovaniPdf()
    {
        Console.Clear();
        UsbPeripheralsMenuVypisy.VypisInfo("--- Generování PDF dokumentu ---");
        
        VypisyDoKonzole.VypisVyzvuKZadani("Cílová složka (např. C:\\Temp nebo cesta k USB): ");
        string? dir = Console.ReadLine();

        VypisyDoKonzole.VypisVyzvuKZadani("Název souboru (např. Potvrzeni): ");
        string? file = Console.ReadLine();

        bool uspech = _pdfGenerator.GeneratePdf(dir ?? "", file ?? "Dokument", "Potvrzení o účasti na školení", "Tímto potvrzujeme, že účastník úspěšně absolvoval kurz.");

        if (uspech)
            UsbPeripheralsMenuVypisy.VypisUspesnouZpravu("PDF dokument byl úspěšně vygenerován.");
        else
            UsbPeripheralsMenuVypisy.VypisChybu("Generování PDF selhalo.");

        Console.WriteLine("\nStiskněte libovolnou klávesu...");
        Console.ReadKey();
    }

    private static void ZpracujSystemoveTiskarny()
    {
        Console.Clear();
        UsbPeripheralsMenuVypisy.VypisInfo("--- Dostupné systémové tiskárny ---");
        var tiskarny = _systemPrinterManager.GetAvailablePrinters();

        if (tiskarny.Count == 0)
        {
            UsbPeripheralsMenuVypisy.VypisChybu("V systému nebyly nalezeny žádné tiskárny.");
        }
        else
        {
            foreach (var t in tiskarny)
            {
                string vychozi = t.IsDefault ? " (VÝCHOZÍ)" : "";
                UsbPeripheralsMenuVypisy.VypisInfo($"-> {t.Name}{vychozi}");
            }
        }

        Console.WriteLine("\nStiskněte libovolnou klávesu...");
        Console.ReadKey();
    }

    private static void ZpracujTiskNaSystemovou()
    {
        Console.Clear();
        UsbPeripheralsMenuVypisy.VypisInfo("--- Tisk na systémovou tiskárnu ---");
        
        VypisyDoKonzole.VypisVyzvuKZadani("Zadejte přesný název tiskárny: ");
        string? printerName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(printerName))
        {
            UsbPeripheralsMenuVypisy.VypisVarovaniVstupu("Název tiskárny nebyl zadaný.");
            Console.ReadKey();
            return;
        }

        bool uspech = _systemPrinterManager.PrintTextToSystemPrinter(printerName, "Testovací tisk prezenční listiny ze systému Správa Školení.");

        if (uspech)
            UsbPeripheralsMenuVypisy.VypisUspesnouZpravu("Úloha byla odeslána do tiskové fronty.");
        else
            UsbPeripheralsMenuVypisy.VypisChybu("Tisk se nezdařil, tiskárna není platná nebo dostupná.");

        Console.ReadKey();
    }

    private static void ZpracujTiskNaSitovou()
    {
        Console.Clear();
        UsbPeripheralsMenuVypisy.VypisInfo("--- Tisk na síťovou tiskárnu (RAW / Port 9100) ---");
        
        VypisyDoKonzole.VypisVyzvuKZadani("IP adresa tiskárny (např. 192.168.1.50): ");
        string? ip = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(ip))
        {
            UsbPeripheralsMenuVypisy.VypisVarovaniVstupu("IP adresa nebyla zadaná.");
            Console.ReadKey();
            return;
        }

        bool uspech = _networkPrinterManager.PrintText(ip, "Test síťového tisku.");

        if (uspech)
            UsbPeripheralsMenuVypisy.VypisUspesnouZpravu("Data byla úspěšně odeslána na síťovou tiskárnu.");
        else
            UsbPeripheralsMenuVypisy.VypisChybu("Připojení k síťové tiskárně vypršelo nebo selhalo.");

        Console.ReadKey();
    }
}