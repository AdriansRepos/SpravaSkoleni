using System.Text.Json;
using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;

namespace SpravaSkoleni.Data.Repozitare;

/// <summary>
/// Datová vrstva pro práci se souborem zamestnanci.json.
/// </summary>
public class ZamestnanciRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private const string SouborZamestnanci = "zamestnanci.json";

    /// <summary>
    /// Uloží lektory a studenty do JSON souboru, každého s příznakem role.
    /// </summary>
    public static void Ulozit(List<Lektor> lektori, List<Student> studenti)
    {
        var vsichni = new List<ZamestnanecZaznam>();

        foreach (var lektor in lektori)
            vsichni.Add(new ZamestnanecZaznam(lektor.Jmeno, lektor.Prijmeni, lektor.Kod!, lektor.PracovniPozice, JeLektorem: true));

        foreach (var student in studenti)
            vsichni.Add(new ZamestnanecZaznam(student.Jmeno, student.Prijmeni, student.Kod!, student.PracovniPozice, JeLektorem: false));

        string json = JsonSerializer.Serialize(vsichni, JsonOptions);
        File.WriteAllText(SouborZamestnanci, json);
    }

    /// <summary>
    /// Načte lektory a studenty z JSON souboru.
    /// </summary>
    public static (List<Lektor> lektori, List<Student> studenti) Nacist()
    {
        var lektori = new List<Lektor>();
        var studenti = new List<Student>();

        if (!File.Exists(SouborZamestnanci))
            return (lektori, studenti);

        try
        {
            string json = File.ReadAllText(SouborZamestnanci);
            var vsichni = JsonSerializer.Deserialize<List<ZamestnanecZaznam>>(json, JsonOptions) ?? [];

            foreach (var zaznam in vsichni)
            {
                if (zaznam.JeLektorem)
                    lektori.Add(new Lektor(zaznam.Jmeno, zaznam.Prijmeni, zaznam.Kod, zaznam.PracovniPozice));
                else
                    studenti.Add(new Student(zaznam.Jmeno, zaznam.Prijmeni, zaznam.Kod, zaznam.PracovniPozice));
            }
        }
        catch (Exception ex)
        {            
            VypisyDoKonzole.VypisVarovani($"Chyba při načítání zamestnanci.json: {ex.Message}");            
        }

        return (lektori, studenti);
    }

    /// <summary>
    /// Pomocný záznam pro JSON serializaci zaměstnance s příznakem role.
    /// </summary>
    internal record ZamestnanecZaznam(string Jmeno, string Prijmeni, string Kod, string PracovniPozice, bool JeLektorem);
}