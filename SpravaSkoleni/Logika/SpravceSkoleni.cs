using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Data.Repozitare;
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
            skoleni.Studenti.Clear();
            skoleni.Lektori.Clear();
            skoleni.HodnoceniStudentu.Clear();
    
            // 1. Dopojení studentů: Studenti patří pod firmu, pro kterou se školení pořádá
            foreach (var kod in skoleni.KodyStudentu)
            {
                var student = NajdiStudentaPodleKoduAFirmy(kod, skoleni.Firma ?? string.Empty);
                if (student != null)
                {
                    skoleni.Studenti.Add(student);
                }
            }
    
            // 2. OPRAVA LEKTORŮ: Lektoři jsou interní/samostatní. 
            // Hledáme je čistě podle jejich vlastního Kódu, protože školení učitelskou firmu nedefinuje.
            foreach (var kod in skoleni.KodyLektoru)
            {
                var lektor = NajdiLektoraPodleKodu(kod);
                if (lektor != null)
                {
                    skoleni.Lektori.Add(lektor);
                }
            }
    
            // 3. OPRAVA DICTIONARY: Použijeme metodu, která vrací čistý, nenullovatelný objekt,
            // čímž okamžitě zmizí chyba s neplatným klíčem v Dictionary.
            foreach (var kvp in skoleni.Hodnoceni)
            {
                var student = NajdiStudentaPodleKoduAFirmy(kvp.Key, skoleni.Firma ?? string.Empty);
                if (student != null)
                {
                    skoleni.HodnoceniStudentu[student] = kvp.Value;
                }
            }
        }
    }       
    
    // Vrátí Student?, protože student v databázi pro danou firmu reálně nemusí existovat
    public Student? NajdiStudentaPodleKoduAFirmy(string kod, string firma) 
        => Studenti.FirstOrDefault(student => student.Kod == kod && student.Firma.Equals(firma, StringComparison.OrdinalIgnoreCase));
    
    // Lektoři se vyhledávají pouze podle unikátního kódu lektora, bez vazby na firmu školení
    public Lektor? NajdiLektoraPodleKodu(string kod) 
        => Lektori.FirstOrDefault(lektor => lektor.Kod == kod);


    public bool PridatSkoleni(Skoleni noveSkoleni)
    {
        if (noveSkoleni.KoncoveDatum < noveSkoleni.PocatecniDatum)
        {                
            VypisyDoKonzole.VypisVarovani("Kritická chyba: Objekt školení má neplatná data!");                
            return false; 
        }
    
        SeznamSkoleni.Add(noveSkoleni);
        UlozitSkoleni();
        return true; 
    }

    // B2B ÚPRAVA: Možnost filtrovat neprihlášené buď globálně (pro externí kurzy), nebo striktně z jedné kmenové firmy
    public List<Student> ZiskejNeprihlaseneStudenty(Skoleni skoleni, string? filtrovatPodleFirmy = null)
    {
        var dotaz = Studenti.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(filtrovatPodleFirmy))
        {
            dotaz = dotaz.Where(s => s.Firma.Equals(filtrovatPodleFirmy, StringComparison.OrdinalIgnoreCase));
        }
        return [.. dotaz.Where(student => !skoleni.Studenti.Contains(student))];
    }

    public List<Lektor> ZiskejNeprirazeneLektory(Skoleni skoleni)
        => [.. Lektori.Where(lektor => !skoleni.Lektori.Contains(lektor))];

    public bool PrihlasStudenta(Skoleni skoleni, Student student)
    {
        if (skoleni.Studenti.Count >= skoleni.DoporucenyPocetUcastniku)
        {
            VypisyDoKonzole.VypisVarovani("[BLOKACE] Kapacita školení je plná!");                
            return false;
        }

        // B2B ÚPRAVA: Překryv se kontroluje napříč všemi kurzy, kde student figuruje
        if (MaStudentPrekryv(student, skoleni.PocatecniDatum, skoleni.KoncoveDatum))
        {                
            VypisyDoKonzole.VypisVarovani($"[BLOKACE] Student {student.Kod} ({student.Firma}) {student.Prijmeni} {student.Jmeno} už v tomto termínu navštěvuje jiné školení!");                
            return false;
        }

        if (!skoleni.Studenti.Contains(student))
        {
            skoleni.Studenti.Add(student);
            skoleni.KodyStudentu.Add(student.Kod!); // Přidáno pro správnou synchronizaci s JSON kolekcí
            UlozitSkoleni();
        }
        return true;
    }

    public void OdhlasStudenta(Skoleni skoleni, Student student)
    {
        if (skoleni.Studenti.Remove(student))
        {
            skoleni.KodyStudentu.Remove(student.Kod!);
            UlozitSkoleni();
        }
    }

    public bool PriradLektora(Skoleni skoleni, Lektor lektor)
    {
        if (MaLektorPrekryv(lektor, skoleni.PocatecniDatum, skoleni.KoncoveDatum))
        {                
            VypisyDoKonzole.VypisVarovani($"[BLOKACE] Lektor {lektor.Kod} ({lektor.Firma}) {lektor.Prijmeni} {lektor.Jmeno} už v tomto termínu vyučuje jiné školení!");                
            return false;
        }

        if (!skoleni.Lektori.Contains(lektor))
        {
            skoleni.Lektori.Add(lektor);
            skoleni.KodyLektoru.Add(lektor.Kod!); // Přidáno pro správnou synchronizaci s JSON kolekcí
            UlozitSkoleni();
        }
        return true;
    }

    public void OdeberLektora(Skoleni skoleni, Lektor lektor)
    {
        if (skoleni.Lektori.Remove(lektor))
        {
            skoleni.KodyLektoru.Remove(lektor.Kod!);
            UlozitSkoleni();
        }
    }

    public void UzavritSkoleni(Skoleni skoleni, Dictionary<Student,int> hodnoceni)
    {
        foreach (var kvp in hodnoceni)
        {
            skoleni.HodnoceniStudentu[kvp.Key] = kvp.Value;
            skoleni.Hodnoceni[kvp.Key.Kod!] = kvp.Value; 
        }
        skoleni.Uzavreno = true;
        UlozitSkoleni();
    }
            
    public IEnumerable<Skoleni> GetSkoleniPodleStavu(bool pouzeOtevrena)
    {
        return SeznamSkoleni.Where(s => s.Uzavreno == !pouzeOtevrena);
    }
    
    public IEnumerable<Skoleni> VyhledatSkoleniPodleTextu(string hledanyText)
    {
        if (string.IsNullOrWhiteSpace(hledanyText))
        {
            return SeznamSkoleni;
        }
    
        string text = hledanyText.Trim().ToLower();
    
        return SeznamSkoleni.Where(s => 
            s.Nazev != null && s.Nazev.Contains(text, StringComparison.OrdinalIgnoreCase) || 
            (s.Lektori.Count > 0 && s.Lektori.Any(l => 
                (l.Prijmeni != null && l.Prijmeni.Contains(text, StringComparison.OrdinalIgnoreCase)) || 
                (l.Kod != null && l.Kod.Contains(text, StringComparison.OrdinalIgnoreCase))))
        );
    }
    
    public IEnumerable<Skoleni> VyhledatSkoleniVRozmeziDat(DateTime odData, DateTime doData)
    {
        return SeznamSkoleni.Where(s => s.PocatecniDatum <= doData && s.KoncoveDatum >= odData);
    }
    
    public IEnumerable<(Student Student, List<Skoleni> SkoleniHistorie)> GetPrehledStudentu()
    {
        var všichniStudenti = SeznamSkoleni
            .SelectMany(s => s.Studenti)
            .GroupBy(st => $"{st.Firma}:{st.Kod}") // B2B ÚPRAVA: Seskupení zohledňuje firmu, aby nesloučilo lidi s jinou firmou a stejným ID
            .Select(g => g.First());

        foreach (var student in všichniStudenti)
        {
            var historie = SeznamSkoleni
                .Where(s => s.Studenti.Any(st => st.Kod == student.Kod && st.Firma == student.Firma))
                .ToList();

            yield return (student, historie);
        }
    }

    public IEnumerable<(Lektor Lektor, List<Skoleni> SkoleniHistorie)> GetPrehledLektoru()
    {
        var všichniLektori = SeznamSkoleni
            .SelectMany(s => s.Lektori)
            .GroupBy(l => $"{l.Firma}:{l.Kod}") // B2B ÚPRAVA: Seskupení podle unikátního složeného identifikátoru
            .Select(g => g.First());

        foreach (var lektor in všichniLektori)
        {
            var historie = SeznamSkoleni
                .Where(s => s.Lektori.Any(l => l.Kod == lektor.Kod && l.Firma == lektor.Firma))
                .ToList();

            yield return (lektor, historie);
        }
    }
    
    private bool MaLektorPrekryv(Lektor lektor, DateTime start, DateTime konec)
    {
        return SeznamSkoleni.Any(s => 
            s.Lektori.Any(l => l.Kod == lektor.Kod && l.Firma == lektor.Firma) && // B2B ÚPRAVA: Přesné vyhledání podle firmy a kódu instance
            start < s.KoncoveDatum && 
            konec > s.PocatecniDatum);
    }
    
    private bool MaStudentPrekryv(Student student, DateTime start, DateTime konec)
    {
        return SeznamSkoleni.Any(s => 
            s.Studenti.Any(st => st.Kod == student.Kod && st.Firma == student.Firma) && // B2B ÚPRAVA: Přesné vyhledání podle firmy a kódu instance
            start < s.KoncoveDatum && 
            konec > s.PocatecniDatum);
    }        
}
