using InputHelper;

namespace SpravaSkoleni
{
    public abstract class Zamestnanec
    {
        private string _jmeno = String.Empty;
        public string Jmeno 
        { 
            get => _jmeno; 
            set => _jmeno = TitleCase.ToTitleCase(value); 
        }
        private string _prijmeni = String.Empty;
        public string Prijmeni 
        { 
            get => _prijmeni; 
            set => _prijmeni = TitleCase.ToTitleCase(value); 
        }
        public string? Kod { get; set; }
        
        public string? PracovniPozice { get; set; }
    }
}