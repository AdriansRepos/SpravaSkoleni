using System.Text.Json.Serialization;

namespace SpravaSkoleni.Entity
{
    public class Student : Zamestnanec
    {
        [JsonIgnore]
        public List<Skoleni> Skoleni { get; set; }

        [JsonConstructor]
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