using SpravaSkoleni.Data.Entity;
using System.Text;
using BarevneVypisyHelper;

namespace SpravaSkoleni.Logika
{
    public static class SpravceExportu
    {
        public static bool ExportujPrezencniListinu(Skoleni skoleni, string slozkaProExport = "Exporty")
        {
            try
            {
                if (!Directory.Exists(slozkaProExport))
                {
                    Directory.CreateDirectory(slozkaProExport);
                }

                string validniNazev = string.Join("_", skoleni.Nazev.Split(Path.GetInvalidFileNameChars()));
                string cestaKSouboru = Path.Combine(slozkaProExport, $"Prezence_{validniNazev}.txt");

                StringBuilder sb = new();
                sb.AppendLine("========================================");
                sb.AppendLine("         PREZENČNÍ LISTINA         ");
                sb.AppendLine("========================================");
                sb.AppendLine($"Název školení: {skoleni.Nazev}");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine("Popis školení:");
                sb.AppendLine($"{skoleni.Popis}");
                sb.AppendLine($"Termín: {skoleni.PocatecniDatum.ToShortDateString()} - {skoleni.KoncoveDatum.ToShortDateString()}");
                                
                var lektoriText = skoleni.Lektori.Count != 0
                    ? string.Join(", ", skoleni.Lektori.Select(l => $"{l.Prijmeni} {l.Jmeno}"))
                    : "Není přiřazen";
                sb.AppendLine($"Lektor: {lektoriText}");
                
                sb.AppendLine($"Kapacita: {skoleni.Studenti.Count} / {skoleni.DoporucenyPocetUcastniku}");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine("Seznam přihlášených studentů:");
                
                if (skoleni.Studenti.Count == 0)
                {
                    sb.AppendLine("  (Žádní studenti nejsou přihlášeni)");
                }
                else
                {
                    int poradi = 1;
                    foreach (var student in skoleni.Studenti)
                    {                        
                        sb.AppendLine($"{poradi}. {student.Prijmeni} {student.Jmeno} (Kód: {student.Kod}) {student.Firma}- [ Podpis: ................... ]");
                        poradi++;
                    }
                }
                sb.AppendLine("========================================");

                File.WriteAllText(cestaKSouboru, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {                
                VypisyDoKonzole.VypisVarovani($"Chyba při exportu: {ex.Message}");                
                return false;
            }
        }
    }
}
