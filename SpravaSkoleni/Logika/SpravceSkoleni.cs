using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Data.Repozitare;
using System.Diagnostics;
using BarevneVypisyHelper;

namespace SpravaSkoleni.Logika;

    public class SpravceSkoleni
    {
        public List<Lektor> Lektori { get; set; } = [];
        public List<Student> Studenti { get; set; } = [];
        public List<Skoleni> SeznamSkoleni { get; set; } = [];
    
        public void UlozitZamestnance() => ZamestnanciRepository.Ulozit(Lektori, Studenti);
        
        public void NacistZamestnance()
        {
            var (lektori, studenti) = ZamestnanciRepository.Nacist();
            Lektori = lektori;
            Studenti = studenti;
        }
    
        public void UlozitSkoleni() => SkoleniRepository.Ulozit(SeznamSkoleni);
        
        public void NacistSkoleni()
        {
            SeznamSkoleni = SkoleniRepository.Nacist(NajdiStudentaPodleKodu, NajdiLektoraPodleKodu);
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

            // 🔥 Pokud má student překryv, dostane stopku!
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
        [Conditional("DEBUG")]
        public void VypisLadiciInfo()
        {
            Console.WriteLine("\n********** Správa školení **********");
            Console.WriteLine($"\nPočet lektorů: {Lektori.Count} \nPočet studentů: {Studenti.Count}");
    
            Console.WriteLine("\n***** Lektoři *****\n");
            foreach (Lektor lektor in Lektori)
            {
                Console.WriteLine($"Jméno: {lektor.Jmeno} {lektor.Prijmeni}");
                Console.WriteLine($"Kód: {lektor.Kod}");
                Console.WriteLine($"Pracovní pozice: {lektor.PracovniPozice}\n");
            }
    
            Console.WriteLine("\n***** Studenti *****\n");
            foreach (Student student in Studenti)
            {
                Console.WriteLine($"Jméno: {student.Jmeno} {student.Prijmeni}");
                Console.WriteLine($"Kód: {student.Kod}");
                Console.WriteLine($"Pracovní pozice: {student.PracovniPozice}\n");
            }
    
            Console.WriteLine("\n***** Školení *****");
            foreach (Skoleni skoleni in SeznamSkoleni)
            {
                Console.WriteLine($"\nNázev školení: {skoleni.Nazev}");
                Console.WriteLine($"Uzavřeno: {(skoleni.Uzavreno ? "ANO" : "NE")}");
                Console.WriteLine($"Popis: {skoleni.Popis}");
                Console.WriteLine($"Doporučený počet účastníků: {skoleni.DoporucenyPocetUcastniku}");
                Console.WriteLine($"Datum konání: {skoleni.PocatecniDatum.ToShortDateString()} - " +
                    $"{skoleni.KoncoveDatum.ToShortDateString()}");
    
                Console.WriteLine("\nLektoři:");
                foreach (Lektor lektor in skoleni.Lektori)
                {
                    Console.WriteLine($"\t{lektor.Jmeno} {lektor.Prijmeni} {lektor.Kod}");
                }
    
                Console.WriteLine("\nStudenti:");
                foreach (Student student in skoleni.Studenti)
                {
                    if (skoleni.Uzavreno)
                    {
                        // Najdeme hodnotu ve slovníku podle kódu studenta, ne podle reference objektu
                        var zaznam = skoleni.HodnoceniStudentu
                            .FirstOrDefault(kvp => kvp.Key.Kod == student.Kod);

                        string textHodnoceni = zaznam.Key != null 
                            ? $"{zaznam.Value}%" 
                            : "Bez hodnocení";

                        Console.WriteLine($"\t{student.Jmeno} {student.Prijmeni} ({student.Kod}) - {textHodnoceni}");
                    }
                    else
                    {
                        Console.WriteLine($"\t{student.Jmeno} {student.Prijmeni} ({student.Kod})");
                    }
                }
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
