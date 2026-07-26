namespace SpravaSkoleni
{
    public class Skoleni(string nazev, string popis, DateTime pocatecniDatum, DateTime koncoveDatum,
        List<Student> studenti, List<Lektor> lektori, Dictionary<Student, int> hodnoceniStudentu,
        int doporucenyPocetUcastniku, bool uzavreno)
    {
        public string Nazev { get; set; } = nazev;
        public string Popis { get; set; } = popis;
        public DateTime PocatecniDatum { get; set; } = pocatecniDatum;
        public DateTime KoncoveDatum { get; set; } = koncoveDatum;
        public List<Student> Studenti { get; set; } = studenti;
        public List<Lektor> Lektori { get; set; } = lektori;
        public Dictionary<Student, int> HodnoceniStudentu { get; set; } = hodnoceniStudentu;
        public int DoporucenyPocetUcastniku { get; set; } = doporucenyPocetUcastniku;
        public bool Uzavreno { get; set; } = uzavreno;
    }
}
