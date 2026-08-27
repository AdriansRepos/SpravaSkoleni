using System.Text.Json.Serialization;
using InputHelper;

namespace SpravaSkoleni.Data.Entity
{
    public abstract class Zamestnanec
    {
        private string _jmeno = string.Empty;
        [JsonPropertyName("jmeno")]
        public string Jmeno
        {
            get => _jmeno;
            set => _jmeno = TitleCase.ToTitleCase(value);
        }

        private string _prijmeni = string.Empty;
        [JsonPropertyName("prijmeni")]
        public string Prijmeni
        {
            get => _prijmeni;
            set => _prijmeni = TitleCase.ToTitleCase(value);
        }

        [JsonPropertyName("kod")]
        public string? Kod { get; set; }

        private string _pracovniPozice = string.Empty;
        [JsonPropertyName("pracovniPozice")]
        public string PracovniPozice
        {
            get => _pracovniPozice;
            set => _pracovniPozice = TitleCase.ToTitleCase(value);
        }
    }
}