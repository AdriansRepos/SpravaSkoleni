using TextHelper;
using SelectHelper;

namespace SpravaSkoleni
{
    public partial class SpravceSkoleni
    {
        public List<Lektor> Lektori { get; set; }
        public List<Student> Studenti { get; set; }
        public List<Skoleni> SeznamSkoleni { get; set; }

        public SpravceSkoleni()
        {
            Lektori = [];
            Studenti = [];
            SeznamSkoleni = [];
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

            Skoleni noveSkoleni = new(nazev, popis, pocatecniDatum, koncoveDatum, doporucenyPocetUcastniku);
            SeznamSkoleni.Add(noveSkoleni);

            UlozitSkoleniJson();
            Console.WriteLine("! Školení bylo úspěšně vytvořeno !");
        }

        public Skoleni? VybratSkoleni()
        {
            Console.WriteLine("\n*** Seznam školení ***\n");

            return SelectHelp.VybratPolozku(
                SeznamSkoleni,
                s => $"{s.Nazev} - {(s.Uzavreno ? "uzavřené" : "otevřené")}",
                "školení");
        }

        public static void ZobrazDetailSkoleni(Skoleni skoleni)
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
            List<Student> studenti = [];
            foreach (Student student in Studenti)
            {
                if (!skoleni.Studenti.Contains(student))
                {
                    studenti.Add(student);
                }
            }

            Console.WriteLine("*** Přihlášení studenta na školení ***");
            Student? vybranyStudent = SelectHelp.VybratPolozku(
                studenti,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "studenta");
            if (vybranyStudent != null)
            {
                skoleni.Studenti.Add(vybranyStudent);
                UlozitSkoleniJson();
                Console.WriteLine("! Student byl úspěšně přihlášen na školení !");
            }
        }

        public void OdhlasitStudenta(Skoleni skoleni)
        {
            Console.WriteLine("*** Odhlášení studenta ze školení ***");
            Student? vybranyStudent = SelectHelp.VybratPolozku(
                skoleni.Studenti,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "studenta");
            if (vybranyStudent != null)
            {
                skoleni.Studenti.Remove(vybranyStudent);
                UlozitSkoleniJson();
                Console.WriteLine("! Student byl úspěšně odhlášen ze školení !");
            }
        }

        public void PriraditLektora(Skoleni skoleni)
        {
            List<Lektor> lektori = [];
            foreach (Lektor lektor in Lektori)
            {
                if (!skoleni.Lektori.Contains(lektor))
                {
                    lektori.Add(lektor);
                }
            }
            Console.WriteLine("*** Přiřazení lektora na školení ***");
            Lektor? vybranyLektor = SelectHelp.VybratPolozku(
                lektori,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "lektora");
            if (vybranyLektor != null)
            {
                skoleni.Lektori.Add(vybranyLektor);
                UlozitSkoleniJson();
                Console.WriteLine("! Lektor byl úspěšně přiřazen na školení !");
            }
        }

        public void OdebratLektora(Skoleni skoleni)
        {
            Console.WriteLine("*** Odebrání lektora ze školení ***");
            Lektor? vybranyLektor = SelectHelp.VybratPolozku(
                skoleni.Lektori,
                s => $"{s.Jmeno} {s.Prijmeni} ({s.Kod})",
                "lektora");
            if (vybranyLektor != null)
            {
                skoleni.Lektori.Remove(vybranyLektor);
                UlozitSkoleniJson();
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
            UlozitSkoleniJson();
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