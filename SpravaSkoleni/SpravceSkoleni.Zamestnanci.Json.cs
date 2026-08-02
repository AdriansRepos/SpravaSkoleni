using System.Text.Json;

namespace SpravaSkoleni
{
    public partial class SpravceSkoleni
    {
        private static readonly JsonSerializerOptions ZamestnanecJsonOptions = new()
        {
            WriteIndented = true
        };

        private const string SouborZamestnanciJson = "zamestnanci.json";

        /// <summary>
        /// Uloží lektory a studenty do JSON souboru, každého s příznakem role.
        /// </summary>
        public void UlozitZamestnanceJson()
        {
            var vsichni = new List<ZamestnanecJsonZaznam>();

            foreach (var lektor in Lektori)
                vsichni.Add(new ZamestnanecJsonZaznam(lektor.Jmeno, lektor.Prijmeni, lektor.Kod!, lektor.PracovniPozice, true));

            foreach (var student in Studenti)
                vsichni.Add(new ZamestnanecJsonZaznam(student.Jmeno, student.Prijmeni, student.Kod!, student.PracovniPozice, false));

            string json = JsonSerializer.Serialize(vsichni, ZamestnanecJsonOptions);
            File.WriteAllText(SouborZamestnanciJson, json);
        }

        /// <summary>
        /// Načte lektory a studenty z JSON souboru.
        /// </summary>
        public void NacistZamestnanceJson()
        {
            if (!File.Exists(SouborZamestnanciJson))
                return;

            try
            {
                string json = File.ReadAllText(SouborZamestnanciJson);
                var vsichni = JsonSerializer.Deserialize<List<ZamestnanecJsonZaznam>>(json, ZamestnanecJsonOptions) ?? [];

                Lektori = [];
                Studenti = [];

                foreach (var zaznam in vsichni)
                {
                    if (zaznam.JeLektorem)
                        Lektori.Add(new Lektor(zaznam.Jmeno, zaznam.Prijmeni, zaznam.Kod, zaznam.PracovniPozice));
                    else
                        Studenti.Add(new Student(zaznam.Jmeno, zaznam.Prijmeni, zaznam.Kod, zaznam.PracovniPozice));
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Chyba při načítání zamestnanci.json: {ex.Message}");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Jednorázová migrace zaměstnanců ze starého textového formátu
        /// (zamestnanci.txt) do nového JSON formátu (zamestnanci.json).
        /// </summary>
        public void MigrujZamestnanceZeStarehoFormatu()
        {
            if (File.Exists(SouborZamestnanciJson))
            {
                Console.WriteLine("zamestnanci.json už existuje, migrace se přeskakuje.");
                return;
            }

            if (!File.Exists("zamestnanci.txt"))
            {
                Console.WriteLine("zamestnanci.txt neexistuje, není co migrovat.");
                return;
            }

            NacistZamestnance();     // stará metoda (txt) – naplní Lektori a Studenti
            UlozitZamestnanceJson(); // nová metoda – zapíše totéž do JSON

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Migrace dokončena: {Lektori.Count} lektorů a {Studenti.Count} studentů převedeno do zamestnanci.json.");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Pomocný záznam pro JSON serializaci zaměstnance s příznakem role
    /// (lektor/student), protože Zamestnanec je abstraktní.
    /// </summary>
    internal record ZamestnanecJsonZaznam(string Jmeno, string Prijmeni, string Kod, string PracovniPozice, bool JeLektorem);
}