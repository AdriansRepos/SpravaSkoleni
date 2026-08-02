using TextHelper;
using SelectHelper;

namespace SpravaSkoleni
{
    public partial class SpravceSkoleni
    {
        public List<Lektor> Lektori { get; set; }
        public List<Student> Studenti { get; set; }
        public List<Skoleni> SeznamSkoleni { get; set; }

        /* Lektory a studenty budeme načítat metodou NacistZamestnance, 
         * proto je nepředáváme parametrem konstruktoru.
         * Školení budeme načítat metodou NacistSkoleni, proto je 
         * také nepředáváme parametrem konstruktoru. */
        public SpravceSkoleni()
        {
            Lektori = [];
            Studenti = [];
            SeznamSkoleni = [];
        }
        
        /* Předpokládáme, že soubor zamestnanci.txt existuje a obsahuje data v očekávaném formátu.
         * Nestandardní situace - např. chybějící údaje, nebo neexistující soubor - neošetřujeme. */
        public void NacistZamestnance()
        {
            StreamReader reader = new("zamestnanci.txt");
            /* Přeskočíme první řádek, protože obsahuje hlavičku a nechceme na základě 
             * něj vytvářet studenta, nebo lektora. */
            reader.ReadLine();
            string radek;
            while ((radek = reader.ReadLine()!) != null)
            {
                string[] hodnoty = radek.Split(';');
                string kod = hodnoty[0];
                string jmeno = hodnoty[1];
                string prijmeni = hodnoty[2];
                string pracovniPozice = hodnoty[3];
                bool jeLektorem = false;

                if (hodnoty[4].Equals("ano", StringComparison.CurrentCultureIgnoreCase))
                {
                    jeLektorem = true;
                }

                if (jeLektorem)
                {
                    Lektor lektor = new(jmeno, prijmeni, kod, pracovniPozice);
                    Lektori.Add(lektor);
                }

                else
                {
                    Student student = new(jmeno, prijmeni, kod, pracovniPozice);
                    Studenti.Add(student);
                }
            }
        }

        public Student? NajdiStudentaPodleKodu(string kod)
        {
            foreach (Student student in Studenti)
            {
                if (student.Kod == kod)
                {
                    return student;
                }
            }
            return null;
        }

        public Lektor? NajdiLektoraPodleKodu(string kod)
        {
            foreach (Lektor lektor in Lektori)
            {
                if (lektor.Kod == kod)
                {
                    return lektor;
                }
            }
            return null;
        }

        public void NoveSkoleni()
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

            DateTime koncoveDatum = UpravaVstupu.ZeptejSeAUprav(
            DateTime.MinValue, "koncové datum (den.měsíc.rok)",
            v => v.ToShortDateString(),
            s => DateTime.Parse(s),
            jeNove: true);
            
            Skoleni noveSkoleni = new(nazev, popis, pocatecniDatum, koncoveDatum, new List<Student>(), 
                new List<Lektor>(), new Dictionary<Student, int>(), doporucenyPocetUcastniku , false);
            SeznamSkoleni.Add(noveSkoleni);

            UlozitSkoleni();
            Console.WriteLine("! Školení bylo úspěšně vytvořeno !");
        }

        public void UlozitSkoleni()
        {
            StreamWriter writer = new("skoleni.txt");
            foreach (Skoleni skoleni in SeznamSkoleni)
            {
                writer.WriteLine("SKOLENI");
                // String.Format - obdoba Console.WriteLine - na daná místa doplní parametry.
                string pocatecniDatum = String.Format($"{skoleni.PocatecniDatum.Day}.{skoleni.PocatecniDatum.Month}." +
                    $"{skoleni.PocatecniDatum.Year}");
                string koncoveDatum = String.Format($"{skoleni.KoncoveDatum.Day}.{skoleni.KoncoveDatum.Month}." +
                    $"{skoleni.KoncoveDatum.Year}");
                string uzavreno = "NE";
                if (skoleni.Uzavreno)
                {
                    uzavreno = "ANO";
                }

                // Zapsání řádku se základními informaceni o školení.
                writer.WriteLine($"{skoleni.Nazev};{skoleni.Popis};{pocatecniDatum};{koncoveDatum};" +
                    $"{skoleni.DoporucenyPocetUcastniku};{uzavreno}");

                // Zapsání seznamu lektorů.
                writer.WriteLine("LEKTORI");
                foreach (Lektor lektor in skoleni.Lektori)
                {
                    writer.WriteLine(lektor.Kod);
                }

                // Zapsání seznamu studentů.
                writer.WriteLine("STUDENTI");
                foreach (Student student in skoleni.Studenti)
                {
                    writer.WriteLine(student.Kod);
                }

                /* Zapsání hodnocení studentů ve formátu
                 * KOD_ZAMESTNANCE;PROCENTUALNI_HODNOCENI. */
                writer.WriteLine("HODNOCENI");
                foreach (Student student in skoleni.HodnoceniStudentu.Keys)
                {
                    writer.WriteLine($"{student.Kod};{skoleni.HodnoceniStudentu[student]}");
                }
            }
            writer.Close();
        }

        public void NacistSkoleni()
        {
            try
            {
                StreamReader reader = new("skoleni.txt");
                // Přeskočíme první řádek souboru - hlavička.
                reader.ReadLine();
                string radek;
                while ((radek = reader.ReadLine()!) != null)
                {
                    /* Přečteme informace o školení, které jsou vždy na řádku, 
                     * který následuje po řádku obsahujícím řetězec SKOLENI. */
                    string[] informaceSkoleni = radek.Split(';');
                    string nazev = informaceSkoleni[0];
                    string popis = informaceSkoleni[1];

                    string[] pocatecniDatumSlozky = informaceSkoleni[2].Split('.');
                    DateTime pocatecniDatum = new(int.Parse(pocatecniDatumSlozky[2]), 
                        int.Parse(pocatecniDatumSlozky[1]), 
                        int.Parse(pocatecniDatumSlozky[0]));

                    string[] koncoveDatumSlozky = informaceSkoleni[3].Split('.');
                    DateTime koncoveDatum = new(int.Parse(koncoveDatumSlozky[2]), 
                        int.Parse(koncoveDatumSlozky[1]), 
                        int.Parse(koncoveDatumSlozky[0]));

                    int doporucenyPocetUcastniku = int.Parse(informaceSkoleni[4]);

                    bool skoleniUzavreno = false;
                    if (informaceSkoleni[5] == "ANO")
                    {
                        skoleniUzavreno = true;                   
                    }

                    // Přeskočíme řádek, na kterém očekáváme, že musí být řetězec LEKTORI.
                    reader.ReadLine();
                    List<Lektor> lektori = [];
                    while ((radek = reader.ReadLine()!) != "STUDENTI")
                    {
                        // Dokud nenarazíme na řádek s řetězcem STUDENTI, čteme lektory.
                        lektori.Add(NajdiLektoraPodleKodu(radek)!);
                    }

                    List<Student> studenti = [];
                    while ((radek = reader.ReadLine()!) != "HODNOCENI")
                    {
                        // Dokud nenarazíme na řádek s řetězcem HODNOCENI, čteme studenty.
                        studenti.Add(NajdiStudentaPodleKodu(radek)!);
                    }

                    Dictionary<Student, int > hodnoceni = [];
                    while (true)
                    {
                        // Dokud nenarazíme na řádek s řetězcem SKOLENI, čteme hodnocení.
                        radek = reader.ReadLine()!;
                        if (radek == null || radek == "SKOLENI")
                            break;
                        string[] hodnotyHodnoceni = radek.Split(';');

                        hodnoceni.Add(NajdiStudentaPodleKodu(hodnotyHodnoceni[0])!, 
                            int.Parse(hodnotyHodnoceni[1]));
                    }

                    /* Vytvoříme si instanci Skoleni. Ta v sobě bude mít informaci, kteří studenti a lektoři se školení účastní.
                     * Zároveň jednotlivým studentům a lektorům přiřadíme informaci o výše vytvořeném školení.
                     * Toto se nám bude hodit ve chvíli, kdy budeme chtít jednoduše získat pro daného 
                     * studenta nebo lektora seznam jeho školení. */
                    Skoleni skoleni = new(nazev, popis, pocatecniDatum, koncoveDatum, studenti, 
                        lektori, hodnoceni, doporucenyPocetUcastniku, skoleniUzavreno);
                    
                     foreach (Student student in studenti)
                     {
                         student.Skoleni.Add(skoleni);
                     }
                     
                     foreach (Lektor lektor in lektori)
                     {
                         lektor.VyucovanaSkoleni.Add(skoleni);
                     }                
                     SeznamSkoleni.Add(skoleni);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání souboru skoleni.txt: {ex.Message}");
                Console.WriteLine("Aplikace bude pokračovat bez načtených školení.");
                Console.ResetColor();

            }
        }

        public Skoleni? VybratSkoleni()
    {
        Console.WriteLine("\n*** Seznam školení ***\n");
    
        return SelectHelp.VybratPolozku(
            SeznamSkoleni,
            s => $"{s.Nazev} - {(s.Uzavreno ? "uzavřené" : "otevřené")}",
            "školení");
    }

        public void ZobrazDetailSkoleni(Skoleni skoleni)
        {
            string stav = "Otevřené";
            if (skoleni.Uzavreno)
            {
                stav = "Uzavřené";
            }

            Console.WriteLine("\n*** Detail školení ***\n");
            Console.WriteLine($"Název školení: {skoleni.Nazev}");
            Console.WriteLine($"Stav: {stav}");
            Console.WriteLine($"Popis: {skoleni.Popis}");
            Console.WriteLine($"Doporučený počet studentů: {skoleni.DoporucenyPocetUcastniku}");
            Console.WriteLine($"Datum konání: {skoleni.PocatecniDatum.ToShortDateString()} - " +
                $"{skoleni.KoncoveDatum.ToShortDateString()}");
            
            Console.WriteLine($"\nLektori:");
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

        public void PrihlasitStudenta(Skoleni skoleni)
        {
            /* Vytvoříme si seznam studentů, kteří ještě na školení nejsou přihlášeni, 
             * a mohou se tedy přihlásit. */
            List<Student> studenti = [];
            foreach (Student student in Studenti)
            {
                if (!skoleni.Studenti.Contains(student))
                {
                    studenti.Add(student);
                }
            }

            Console.WriteLine("*** Přihlášení studenta na školení ***");
            /* Zobrazíme seznam studentů, které je možné přihlásit na školení a 
             * necháme uživatele vybrat, kterého chce přihlásit. */
            Student? vybranyStudent = SelectHelp.VybratPolozku(
                studenti,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "studenta");
            if (vybranyStudent != null)
            {
                skoleni.Studenti.Add(vybranyStudent);
                UlozitSkoleni();
                Console.WriteLine("! Student byl úspěšně přihlášen na školení !");
            }
        }

        public void OdhlasitStudenta(Skoleni skoleni)
        {
            Console.WriteLine("*** Odhhlášení studenta ze školení ***");
            /* Zobrazíme seznam studentů, které je možné odhlásit ze školení a 
             * necháme uživatele vybrat, kterého chce odhlásit. */
            Student? vybranyStudent = SelectHelp.VybratPolozku(
                skoleni.Studenti,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "studenta");
            if (vybranyStudent != null)            
            {
                skoleni.Studenti.Remove(vybranyStudent);
                UlozitSkoleni();
                Console.WriteLine("! Student byl úspěšně odhlášen ze školení !");
            }
        }

        public void PriraditLektora(Skoleni skoleni)
        {
            /* Vvytvoříme si seznam lektorů, kteří ještě nejsou přiřazeni na toto školení a 
             * mohou ted být přiřazeni */
            List<Lektor> lektori = [];
            foreach (Lektor lektor in Lektori)
            {
                if (!skoleni.Lektori.Contains(lektor))
                {
                    lektori.Add(lektor);
                }
            }
            Console.WriteLine("*** Přiřazení lektora na školení ***");
            /* Zobrazíme seznam lektorů, které je možné přiřadit na školení a 
             * necháme uživatele vybrat, kterého chce přiřadit. */
            Lektor? vybranyLektor = SelectHelp.VybratPolozku(
                lektori,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "lektora");
            if (vybranyLektor != null)
            {
                skoleni.Lektori.Add(vybranyLektor);
                UlozitSkoleni();
                Console.WriteLine("! Lektor byl úspěšně přiřazen na školení !");
            }
        }

        public void OdebratLektora(Skoleni skoleni)
        {
            Console.WriteLine("*** Odebraání lektora na školení ***");
            /* Zobrazíme seznam lektorů, které je možné odebrat ze školení a 
             * necháme uživatele vybrat, kterého chce odebrat. */
             Lektor? vybranyLektor = SelectHelp.VybratPolozku(
                skoleni.Lektori,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "lektora");
            if (vybranyLektor != null) 
            {
                skoleni.Lektori.Remove(vybranyLektor);
                UlozitSkoleni();
                Console.WriteLine("! Lektor byl úspěšně odebrán na školení !");
            }
        }

        public void UzavritSkoleni(Skoleni skoleni)
        {
            Console.WriteLine("*** Uzavření školení ***");
            Console.WriteLine("Vyplňte hodnocení jednotlivých studentů");
            foreach (Student student in skoleni.Studenti)
            {
                int hodnoceni = UpravaVstupu.ZeptejSeAUprav(
                    0, $"hodnocení pro {student.Jmeno} {student.Prijmeni}",
                    v => v.ToString(),
                    s => int.Parse(s),
                    jeNove: true);
                skoleni.HodnoceniStudentu[student] = hodnoceni;
            }
            skoleni.Uzavreno = true;
            UlozitSkoleni();
            Console.WriteLine("! Školení bylo úspěšně uzavřeno !");
        }

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
                        Console.WriteLine($"\t{student.Jmeno} {student.Prijmeni} ({student.Kod}) - " +
                            $"{skoleni.HodnoceniStudentu[student]}%");
                    }
                    else
                    {
                        Console.WriteLine($"\t{student.Jmeno} {student.Prijmeni} ({student.Kod})");
                    }
                }
            }
        }
    }
}