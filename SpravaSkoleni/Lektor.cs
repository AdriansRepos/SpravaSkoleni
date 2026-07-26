namespace SpravaSkoleni
{
    public class Lektor : Zamestnanec
    {
        public List<Skoleni> VyucovanaSkoleni { get; set; }

        public Lektor(string jmeno, string prijmeni, string kod, string pracovniPozice)
        {
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            Kod = kod;
            PracovniPozice = pracovniPozice;
            VyucovanaSkoleni = [];
        }
    }
}
