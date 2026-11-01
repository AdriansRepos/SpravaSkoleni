using TextHelper;
using SelectHelper;
using BarevneVypisyHelper;
using SpravaSkoleni.Logika;
using SpravaSkoleni.Data.Entity;

namespace SpravaSkoleni.Vypisy
{
    public static class SkoleniVypisy
    {
        public static async Task VytvoritNoveSkoleniAsync(SpravceSkoleni spravce)
        {
            Console.WriteLine("\n*** Vytvoření nového školení ***\n");
            Console.Write("Název: ");
            string nazev = Console.ReadLine()!;
            Console.Write("Popis: ");
            string popis = Console.ReadLine()!;
        
            int doporucenyPocetUcastniku = UpravaVstupu.ZeptejSeAUprav(
                0, "doporučený počet účastníků",
                v => v.ToString(),
                s => int.Parse(s),
                jeNove: true);
        
            DateTime pocatecniDatum = UpravaVstupu.ZeptejSeAUprav(
                DateTime.MinValue, "počáteční datum (den.měsíc.rok)",
                v => v.ToShortDateString(),
                s => DateTime.Parse(s),
                jeNove: true);
        
            // TADY ZAČÍNÁ CYKLUS PRO KONTROLU ZADÁVANÝCH DATUMŮ:
            DateTime koncoveDatum;
            while (true)
            {
                koncoveDatum = UpravaVstupu.ZeptejSeAUprav(
                    DateTime.MinValue, "koncové datum (den.měsíc.rok)",
                    v => v.ToShortDateString(),
                    s => DateTime.Parse(s),
                    jeNove: true);
        
                // Pokud je konec po začátku (nebo ve stejný den), vše je OK a končíme
                if (koncoveDatum >= pocatecniDatum)
                {
                    break; 
                }                       
                
                VypisyDoKonzole.VypisVarovani("\n[CHYBA] Koncové datum nemůže být před počátečním datumem!");
                VypisyDoKonzole.VypisInformaci($"Školení začíná {pocatecniDatum.ToShortDateString()}, zadejte správný datum konce.\n");                
            }
        
            // Teprve až projdeme kontrolou, vytvoříme bezpečný objekt
            Skoleni noveSkoleni = new(nazev, popis, pocatecniDatum, koncoveDatum, doporucenyPocetUcastniku, "");
            try
            {
                await spravce.PridatSkoleniAsync(noveSkoleni);
                VypisyDoKonzole.VypisUspech("! Školení bylo úspěšně vytvořeno !");
            }
            catch (ArgumentException ex)
            {
                VypisyDoKonzole.VypisVarovani(ex.Message);
            }            
        }

        public static Skoleni? VybratSkoleni(SpravceSkoleni spravce)
        {
            Console.WriteLine("\n*** Seznam školení ***\n");

            return SelectHelp.VybratPolozku(
                spravce.SeznamSkoleni,
                s => $"{s.Nazev} - {(s.Uzavreno ? "uzavřené" : "otevřené")}",
                "školení");
        }

        public static void ZobrazDetailSkoleni(Skoleni skoleni)
        {
            string stav = skoleni.Uzavreno ? "Uzavřené" : "Otevřené";

            Console.WriteLine("\n*** Detail školení ***\n");
            Console.WriteLine($"Název školení: {skoleni.Nazev}");
            Console.WriteLine($"Stav: {stav}");
            Console.WriteLine($"Popis: {skoleni.Popis}");
            Console.WriteLine($"Doporučený počet studentů: {skoleni.DoporucenyPocetUcastniku}");
            Console.WriteLine($"Datum konání: {skoleni.PocatecniDatum.ToShortDateString()} - " +
                              $"{skoleni.KoncoveDatum.ToShortDateString()}");

            Console.WriteLine($"\nLektoři:");
            foreach (Lektor lektor in skoleni.Lektori)
            {
                Console.WriteLine($"\t{lektor.Jmeno} {lektor.Prijmeni} ({lektor.Kod})");
            }

            Console.WriteLine("\nStudenti:");
            foreach (Student student in skoleni.Studenti)
            {
                if (skoleni.Uzavreno)
                {
                    Console.WriteLine($"\t{student.Jmeno} {student.Prijmeni} ({student.Kod}) - " +
                                      $"{skoleni.HodnoceniStudentu[student]}%");
                }
                else
                {
                    Console.WriteLine($"\t{student.Jmeno} {student.Prijmeni} ({student.Kod})");
                }
            }
        }

        public static async Task PrihlasitStudenta(SpravceSkoleni spravce, Skoleni skoleni)
        {
            var neprihlaseniStudenti = spravce.ZiskejNeprihlaseneStudenty(skoleni);

            Console.WriteLine("*** Přihlášení studenta na školení ***");
            Student? vybranyStudent = SelectHelp.VybratPolozku(
                neprihlaseniStudenti,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "studenta");

            if (vybranyStudent != null)
            {
                try
                {
                    await spravce.PrihlasStudentaAsync(skoleni, vybranyStudent);
                    VypisyDoKonzole.VypisUspech("! Student byl úspěšně přihlášen na školení !");
                }
                catch (InvalidOperationException ex)
                {
                    // Chytáme byznys chybu z logiky a barevně ji vypisujeme v UI vrstvě
                    VypisyDoKonzole.VypisVarovani($"[BLOKACE] {ex.Message}");
                }
            }
        }

        public static async Task OdhlasitStudenta(SpravceSkoleni spravce, Skoleni skoleni)
        {
            Console.WriteLine("*** Odhlášení studenta ze školení ***");
            Student? vybranyStudent = SelectHelp.VybratPolozku(
                skoleni.Studenti,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "studenta");

            if (vybranyStudent != null)
            {
                await spravce.OdhlasStudentaAsync(skoleni, vybranyStudent);
                VypisyDoKonzole.VypisUspech("! Student byl úspěšně odhlášen ze školení !");
            }
        }

        public static async Task PriraditLektora(SpravceSkoleni spravce, Skoleni skoleni)
        {
            var neprirazeniLektori = spravce.ZiskejNeprirazeneLektory(skoleni);

            Console.WriteLine("*** Přiřazení lektora na školení ***");
            Lektor? vybranyLektor = SelectHelp.VybratPolozku(
                neprirazeniLektori,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "lektora");

            if (vybranyLektor != null)
            {
                try
                {
                    await spravce.PriradLektoraAsync(skoleni, vybranyLektor);
                    VypisyDoKonzole.VypisUspech("! Lektor byl úspěšně přiřazen na školení !");
                }
                catch (InvalidOperationException ex)
                {
                    VypisyDoKonzole.VypisVarovani($"[BLOKACE] {ex.Message}");
                }
            }
        }

        public static async Task OdebratLektora(SpravceSkoleni spravce, Skoleni skoleni)
        {
            Console.WriteLine("*** Odebrání lektora ze školení ***");
            Lektor? vybranyLektor = SelectHelp.VybratPolozku(
                skoleni.Lektori,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "lektora");

            if (vybranyLektor != null)
            {
                await spravce.OdeberLektoraAsync(skoleni, vybranyLektor);
                VypisyDoKonzole.VypisUspech("! Lektor byl úspěšně odebrán ze školení !");
            }
        }

        public static async Task UzavritSkoleni(SpravceSkoleni spravce, Skoleni skoleni)
        {
            Console.WriteLine("*** Uzavření školení ***");
            Console.WriteLine("Vyplňte hodnocení jednotlivých studentů");

            Dictionary<Student, int> hodnoceni = [];
            foreach (Student student in skoleni.Studenti)
            {
                int h = UpravaVstupu.ZeptejSeAUprav(
                    0, $"hodnocení pro {student.Jmeno} {student.Prijmeni}",
                    v => v.ToString(),
                    s => int.Parse(s),
                    jeNove: true);
                
                hodnoceni[student] = h;
            }

            await spravce.UzavritSkoleniAsync(skoleni, hodnoceni);
            VypisyDoKonzole.VypisUspech("! Školení bylo úspěšně uzavřeno !");
        }
    }
}
