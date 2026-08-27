using System.Text.Json.Serialization;

namespace SpravaSkoleni.Data.Entity
{
    public class Lektor : Zamestnanec
    {
        [JsonIgnore]
        public List<Skoleni> VyucovanaSkoleni { get; set; }

        [JsonConstructor]
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