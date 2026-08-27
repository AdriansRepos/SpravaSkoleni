using System.Text.Json.Serialization;

namespace SpravaSkoleni.Data.Entity
{
    [method: JsonConstructor]
    public class Skoleni(string nazev, string popis, DateTime pocatecniDatum, DateTime koncoveDatum,
        List<string> kodyStudentu, List<string> kodyLektoru, Dictionary<string, int> hodnoceni,
        int doporucenyPocetUcastniku, bool uzavreno)
    {
        [JsonPropertyName("nazev")]
        public string Nazev { get; set; } = nazev;

        [JsonPropertyName("popis")]
        public string Popis { get; set; } = popis;

        [JsonPropertyName("pocatecniDatum")]
        public DateTime PocatecniDatum { get; set; } = pocatecniDatum;

        [JsonPropertyName("koncoveDatum")]
        public DateTime KoncoveDatum { get; set; } = koncoveDatum;

        /// <summary>
        /// Kódy přihlášených studentů. Skutečné objekty Student se
        /// dopojí po načtení podle NajdiStudentaPodleKodu.
        /// </summary>
        [JsonPropertyName("kodyStudentu")]
        public List<string> KodyStudentu { get; set; } = kodyStudentu;

        /// <summary>
        /// Kódy přiřazených lektorů. Skutečné objekty Lektor se
        /// dopojí po načtení podle NajdiLektoraPodleKodu.
        /// </summary>
        [JsonPropertyName("kodyLektoru")]
        public List<string> KodyLektoru { get; set; } = kodyLektoru;

        /// <summary>
        /// Hodnocení studentů ve formátu kód -> procento.
        /// </summary>
        [JsonPropertyName("hodnoceni")]
        public Dictionary<string, int> Hodnoceni { get; set; } = hodnoceni;

        [JsonPropertyName("doporucenyPocetUcastniku")]
        public int DoporucenyPocetUcastniku { get; set; } = doporucenyPocetUcastniku;

        [JsonPropertyName("uzavreno")]
        public bool Uzavreno { get; set; } = uzavreno;

        /// <summary>
        /// Skutečné objekty studentů, dopojené po načtení ze souboru
        /// podle KodyStudentu. Není serializováno přímo.
        /// </summary>
        [JsonIgnore]
        public List<Student> Studenti { get; set; } = [];

        /// <summary>
        /// Skutečné objekty lektorů, dopojené po načtení ze souboru
        /// podle KodyLektoru. Není serializováno přímo.
        /// </summary>
        [JsonIgnore]
        public List<Lektor> Lektori { get; set; } = [];

        /// <summary>
        /// Hodnocení studentů podle skutečného objektu Student,
        /// dopojené po načtení. Není serializováno přímo.
        /// </summary>
        [JsonIgnore]
        public Dictionary<Student, int> HodnoceniStudentu { get; set; } = [];

        /// <summary>
        /// Vytvoří nové, prázdné školení pro zápis do paměti (bez
        /// nutnosti zadávat kódy přímo).
        /// </summary>
        public Skoleni(string nazev, string popis, DateTime pocatecniDatum, DateTime koncoveDatum,
            int doporucenyPocetUcastniku)
            : this(nazev, popis, pocatecniDatum, koncoveDatum, [], [], 
                  [], doporucenyPocetUcastniku, false)
        {
        }
    }
}