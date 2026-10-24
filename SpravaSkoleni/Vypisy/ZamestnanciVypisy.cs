using SpravaSkoleni.Logika;

namespace SpravaSkoleni.Vypisy
{
    public static class ZamestnanciVypisy
    {
        public static void VypisVsechnyStudenty(SpravceSkoleni spravce)
        {
            Console.WriteLine("\n*** PŘEHLED STUDENTŮ A JEJICH ŠKOLENÍ ***\n");
            var prehled = spravce.GetPrehledStudentu();
            int pocitadlo = 0;

            foreach (var (Student, SkoleniHistorie) in prehled)
            {
                pocitadlo++;
                Console.WriteLine($"{pocitadlo}. Student: {Student.Prijmeni} {Student.Jmeno} [{Student.Kod}]");
                Console.WriteLine("   Absolvovaná/plánovaná školení:");
                
                if (SkoleniHistorie.Count == 0)
                {
                    Console.WriteLine("     - Žádná registrovaná školení");
                }
                else
                {
                    foreach (var skoleni in SkoleniHistorie)
                    {
                        string stav = skoleni.Uzavreno ? "Uzavřené" : "Otevřené";
                        Console.WriteLine($"     • {skoleni.Nazev} ({skoleni.PocatecniDatum.ToShortDateString()}) - [{stav}]");
                    }
                }
                Console.WriteLine(new string('-', 50));
            }

            if (pocitadlo == 0)
            {
                Console.WriteLine("V systému momentálně nejsou žádní studenti přiřazení na školení.");
            }
        }

        public static void VypisVsechnyLektory(SpravceSkoleni spravce)
        {
            Console.WriteLine("\n*** PŘEHLED LEKTORŮ A JEJICH ŠKOLENÍ ***\n");
            var prehled = spravce.GetPrehledLektoru();
            int pocitadlo = 0;

            foreach (var (Lektor, SkoleniHistorie) in prehled)
            {
                pocitadlo++;
                Console.WriteLine($"{pocitadlo}. Lektor: {Lektor.Prijmeni} {Lektor.Jmeno} [{Lektor.Kod}]");
                Console.WriteLine("   Vyučovaná školení:");
                
                if (SkoleniHistorie.Count == 0)
                {
                    Console.WriteLine("     - Žádná přiřazená školení");
                }
                else
                {
                    foreach (var skoleni in SkoleniHistorie)
                    {
                        string stav = skoleni.Uzavreno ? "Uzavřené" : "Otevřené";
                        Console.WriteLine($"     • {skoleni.Nazev} ({skoleni.PocatecniDatum.ToShortDateString()}) - [{stav}]");
                    }
                }
                Console.WriteLine(new string('-', 50));
            }

            if (pocitadlo == 0)
            {
                Console.WriteLine("V systému momentálně nejsou žádní letoři přiřazení na školení.");
            }
        }
    }
}
