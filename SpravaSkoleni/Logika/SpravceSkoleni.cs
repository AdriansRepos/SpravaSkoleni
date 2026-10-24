using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Data.Repozitare;
using System.Diagnostics;
using BarevneVypisyHelper;

namespace SpravaSkoleni.Logika;

public class SpravceSkoleni
{
    private readonly SkoleniRepository _skoleniRepo = new();
    private readonly StudentiRepository _studentiRepo = new();
    private readonly LektoriRepository _lektoriRepo = new();

    public List<Lektor> Lektori { get; set; } = [];
    public List<Student> Studenti { get; set; } = [];
    public List<Skoleni> SeznamSkoleni { get; set; } = [];

    // Konstruktor, který automaticky zajistí načtení dat a dopojení vazeb při startu aplikace
    public SpravceSkoleni()
    {
        NacistVsechnaData();
    }

    public void UlozitStudenta() => _studentiRepo.Ulozit(Studenti);
    public void UlozitLektora() => _lektoriRepo.Ulozit(Lektori);
    public void UlozitSkoleni() => _skoleniRepo.Ulozit(SeznamSkoleni);
    
    public void NacistVsechnaData()
    {
        // Správné instanční načtení z tvých nových rozdělených repozitářů
        Lektori = _lektoriRepo.NacistVsechny();
        Studenti = _studentiRepo.NacistVsechny();
        SeznamSkoleni = _skoleniRepo.Nacist();
        
        DopojReference(); 
    }

    /// <summary>
    /// Propojí uložené textové kódy z JSONu se skutečnými instancemi objektů v paměti (LINQ).
    /// </summary>
    private void DopojReference()
    {
        foreach (var skoleni in SeznamSkoleni)
        {
            // Vyčistíme staré reference před novým naplněním
            skoleni.Studenti.Clear();
            skoleni.Lektori.Clear();
            skoleni.HodnoceniStudentu.Clear();

            // Dopojení studentů podle kódů
            foreach (var kod in skoleni.KodyStudentu)
            {
                var student = NajdiStudentaPodleKodu(kod);
                if (student != null)
                {
                    skoleni.Studenti.Add(student);
                }
            }

            // Dopojení lektorů podle kódů
            foreach (var kod in skoleni.KodyLektoru)
            {
                var lektor = NajdiLektoraPodleKodu(kod);
                if (lektor != null)
                {
                    skoleni.Lektori.Add(lektor);
                }
            }

            // Dopojení hodnocení
            foreach (var kvp in skoleni.Hodnoceni)
            {
                var student = NajdiStudentaPodleKodu(kvp.Key);
                if (student != null)
                {
                    skoleni.HodnoceniStudentu[student] = kvp.Value;
                }
            }
        }
    }       
    
    public Student? NajdiStudentaPodleKodu(string kod) 
        => Studenti.FirstOrDefault(student => student.Kod == kod);

    public Lektor? NajdiLektoraPodleKodu(string kod) 
        => Lektori.FirstOrDefault(lektor => lektor.Kod == kod);

    public bool PridatSkoleni(Skoleni noveSkoleni)
    {
        if (noveSkoleni.KoncoveDatum < noveSkoleni.PocatecniDatum)
        {                
            VypisyDoKonzole.VypisVarovani("Kritická chyba: Objekt školení má neplatná data!");                
            return false; // Zmetek nepřidáme!
        }
    
        SeznamSkoleni.Add(noveSkoleni);
        UlozitSkoleni();
        return true; // Vše klaplo
    }

    public List<Student> ZiskejNeprihlaseneStudenta(Skoleni skoleni)
        => [.. Studenti.Where(student => !skoleni.Studenti.Contains(student))];

    public List<Lektor> ZiskejNeprirazeneLektory(Skoleni skoleni)
        => [.. Lektori.Where(lektor => !skoleni.Lektori.Contains(lektor))];

    public bool PrihlasStudenta(Skoleni skoleni, Student student)
    {
        // Kontrola kapacity (kterou už máš v plánu)
        if (skoleni.Studenti.Count >= skoleni.DoporucenyPocetUcastniku)
        {
            VypisyDoKonzole.VypisVarovani("[BLOKACE] Kapacita školení je plná!");                
            return false;
        }

        // Pokud má student překryv, dostane stopku!
        if (MaStudentPrekryv(student, skoleni.PocatecniDatum, skoleni.KoncoveDatum))
        {                
            VypisyDoKonzole.VypisVarovani($"[BLOKACE] Student {student.Kod} {student.Prijmeni} {student.Jmeno} už v tomto termínu navštěvuje jiné školení!");                ;
            return false;
        }

        if (!skoleni.Studenti.Contains(student))
        {
            skoleni.Studenti.Add(student);
            UlozitSkoleni();
        }
        return true;
    }

    public void OdhlasStudenta(Skoleni skoleni, Student student)
    {
        if (skoleni.Studenti.Remove(student))
        {
            UlozitSkoleni();
        }
    }

    public bool PriradLektora(Skoleni skoleni, Lektor lektor)
    {
        // Pokud má lektor překryv, zablokujeme to a nepustíme ho dál!
        if (MaLektorPrekryv(lektor, skoleni.PocatecniDatum, skoleni.KoncoveDatum))
        {                
            VypisyDoKonzole.VypisVarovani($"[BLOKACE] Lektor {lektor.Kod} {lektor.Prijmeni} {lektor.Jmeno} už v tomto termínu vyučuje jiné školení!");                
            return false;
        }

        if (!skoleni.Lektori.Contains(lektor))
        {
            skoleni.Lektori.Add(lektor);
            UlozitSkoleni();
        }
        return true;
    }

    public void OdeberLektora(Skoleni skoleni, Lektor lektor)
    {
        if (skoleni.Lektori.Remove(lektor))
        {
            UlozitSkoleni();
        }
    }

    public void UzavritSkoleni(Skoleni skoleni, Dictionary<Student,int> hodnoceni)
    {
        foreach (var kvp in hodnoceni)
        {
            skoleni.HodnoceniStudentu[kvp.Key] = kvp.Value;
            skoleni.Hodnoceni[kvp.Key.Kod!] = kvp.Value; // zajistí persistenci
        }
        skoleni.Uzavreno = true;
        UlozitSkoleni();
    }
            
    // ==========================================
    // 1. FILTROVÁNÍ PODLE STAVU 
    // ==========================================
    public IEnumerable<Skoleni> GetSkoleniPodleStavu(bool pouzeOtevrena)
    {
        // Pokud chceme pouze otevřená, hledáme s.Uzavreno == false
        return SeznamSkoleni.Where(s => s.Uzavreno == !pouzeOtevrena);
    }
    
    // ==========================================
    // 2. VYHLEDÁVÁNÍ PODLE TEXTU (Název / Lektor)
    // ==========================================
    public IEnumerable<Skoleni> VyhledatSkoleniPodleTextu(string hledanyText)
    {
        if (string.IsNullOrWhiteSpace(hledanyText))
        {
            return SeznamSkoleni;
        }
    
        string text = hledanyText.Trim().ToLower();
    
        // Hledá shodu v názvu ŠKOLENÍ nebo v PŘÍJMENÍ / KÓDU jakéhokoliv lektora
        return SeznamSkoleni.Where(s => 
            s.Nazev != null && s.Nazev.Contains(text, StringComparison.OrdinalIgnoreCase) || 
            (s.Lektori.Count > 0 && s.Lektori.Any(l => 
                (l.Prijmeni != null && l.Prijmeni.Contains(text, StringComparison.OrdinalIgnoreCase)) || 
                (l.Kod != null && l.Kod.Contains(text, StringComparison.OrdinalIgnoreCase))))
        );
    }
    
    // ==========================================
    // 3. VYHLEDÁVÁNÍ V ROZMEZÍ DATUMŮ
    // ==========================================
    public IEnumerable<Skoleni> VyhledatSkoleniVRozmeziDat(DateTime odData, DateTime doData)
    {
        // Neprůstřelný překryv časových os
        return SeznamSkoleni.Where(s => 
            s.PocatecniDatum <= doData && s.KoncoveDatum >= odData
        );
    }
    
    // 1. Získání přehledu všech studentů a jejich školení
    public IEnumerable<(Student Student, List<Skoleni> SkoleniHistorie)> GetPrehledStudentu()
    {
        // Vytáhneme všechny unikátní studenty, kteří jsou na nějakém školení
        var všichniStudenti = SeznamSkoleni
            .SelectMany(s => s.Studenti)
            .GroupBy(st => st.Kod) // Seskupíme podle Kódu
            .Select(g => g.First());

        foreach (var student in všichniStudenti)
        {
            // Najdeme všechna školení, kde tento student figuruje
            var historie = SeznamSkoleni
                .Where(s => s.Studenti.Any(st => st.Kod == student.Kod))
                .ToList();

            yield return (student, historie);
        }
    }

    // 2. Získání přehledu všech lektorů a jejich školení
    public IEnumerable<(Lektor Lektor, List<Skoleni> SkoleniHistorie)> GetPrehledLektoru()
    {
        // Vytáhneme všechny unikátní lektory
        var všichniLektori = SeznamSkoleni
            .SelectMany(s => s.Lektori)
            .GroupBy(l => l.Kod)
            .Select(g => g.First());

        foreach (var lektor in všichniLektori)
        {
            // Najdeme všechna školení, která tento lektor vyučuje
            var historie = SeznamSkoleni
                .Where(s => s.Lektori.Any(l => l.Kod == lektor.Kod))
                .ToList();

            yield return (lektor, historie);
        }
    }
    
    // 1. Pomocná pojistka pro lektora
    private bool MaLektorPrekryv(Lektor lektor, DateTime start, DateTime konec)
    {
        // Hledáme v seznamu, zda lektor už neučí školení, které se překrývá s novým časem
        return SeznamSkoleni.Any(s => 
            s.Lektori.Contains(lektor) && 
            start < s.KoncoveDatum && 
            konec > s.PocatecniDatum);
    }
    
    // 2. Pomocná pojistka pro studenta
    private bool MaStudentPrekryv(Student student, DateTime start, DateTime konec)
    {
        // Hledáme, zda student už nemá v tomto čase jiné registrované školení
        return SeznamSkoleni.Any(s => 
            s.Studenti.Contains(student) && 
            start < s.KoncoveDatum && 
            konec > s.PocatecniDatum);
    }        
}