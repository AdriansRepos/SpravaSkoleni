using System.Text.Json.Serialization;

namespace SpravaSkoleni.Data.Entity
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

        public override bool Equals(object? obj)
        {
            if (obj is Student druhy)
                return Kod == druhy.Kod;
            return false;
        }
        
        public override int GetHashCode()
        {
            return Kod?.GetHashCode() ?? 0;
        }
    }
}