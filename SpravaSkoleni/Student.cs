namespace SpravaSkoleni
{
    public class Student : Zamestnanec
    {
        public List<Skoleni> Skoleni { get; set; }

        public Student(string jmeno, string prijmeni, string kod, string pracovniPozice)
        {
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            Kod = kod;
            PracovniPozice = pracovniPozice;
            Skoleni = [];
        }
    }
}
