using System.Runtime.Versioning;
using UsbPeripheralsLib.Documents;
using UsbPeripheralsLib.Printing;
using UsbPeripheralsLib.Storage;
using SpravaSkoleni.Enumy;

namespace SpravaSkoleni.Menu;

[SupportedOSPlatform("windows")]
public class UsbPeripheralsMenu
{
    private static readonly UsbManager _usbManager = new();
    private static readonly PdfDocumentGenerator _pdfGenerator = new();
    private static readonly SystemPrinterManager _systemPrinterManager = new();
    private static readonly NetworkPrinterManager _networkPrinterManager = new();

    public static UsbMenuVolba ZpracujVolbu(string vstup)
    {
        return vstup.Trim() switch
        {
            "1" => UsbMenuVolba.DostupneDisky,
            "2" => UsbMenuVolba.ExportNaUsb,
            "3" => UsbMenuVolba.GenerovatPdf,
            "4" => UsbMenuVolba.SystemoveTiskarny,
            "5" => UsbMenuVolba.TiskNaSystemovou,
            "6" => UsbMenuVolba.TiskNaSitovou,
            "0" => UsbMenuVolba.NavratZpet,
            _ => UsbMenuVolba.NeplatnaVolba
        };
    }

    // Metody obsluhující byznysovou logiku a volání externí knihovny
    public static List<UsbDriveModel> ZiskejDisky() => [.. _usbManager.GetConnectedUsbDrives()];

    public static bool ProvedExport(string drive, string fileName)
    {
        string obsah = "Automatická záloha dat ze systému SpravaSkoleni vytvořena dne " + DateTime.Now;
        return _usbManager.ExportFileToUsb(drive, fileName, obsah);
    }

    public static bool ProvedPdf(string dir, string file)
    {
        return _pdfGenerator.GeneratePdf(dir, file, "Potvrzení o účasti na školení", "Tímto potvrzujeme, že účastník úspěšně absolvoval kurz.");
    }

    public static List<PrinterInfo> ZiskejSystemoveTiskarny() => _systemPrinterManager.GetAvailablePrinters();

    public static bool TiskSystem(string printerName)
    {
        return _systemPrinterManager.PrintTextToSystemPrinter(printerName, "Testovací tisk prezenční listiny ze systému Správa Školení.");
    }

    public static bool TiskSit(string ip)
    {
        return _networkPrinterManager.PrintText(ip, "Test síťového tisku.");
    }
}