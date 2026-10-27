using BarevneVypisyHelper;
using SpravaSkoleni.Data.Entity;
using SpravaSkoleni.Zaloha;
using System.Text.Json;

namespace SpravaSkoleni.Data.Repozitare;
public class StudentiRepository(string slozkaStudenti = "Studenti")
{    
    private readonly string _slozkaStudenti = slozkaStudenti;
        
    private readonly string _cestaStudenti = Path.Combine(slozkaStudenti, "studenti.json");
    
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        WriteIndented = true 
    };

    // Pomocný datový záznam pro čisté ukládání (DTO)
    private record ZamestnanecZaznam(string Jmeno, string Prijmeni, string Kod, string PracovniPozice, string Firma, bool JeLektorem);

    public List<Student> NacistVsechny()
    {        
        var studenti = new List<Student>();
                
        // 1. Tichá obnova studentů ze zálohy
        string zalohaStudenti = Path.Combine("Zalohy", "studenti.bak.json");
        if (!File.Exists(_cestaStudenti) && File.Exists(zalohaStudenti))
        {
            try 
            { 
                Directory.CreateDirectory(_slozkaStudenti); 
                File.Copy(zalohaStudenti, _cestaStudenti); 
            }
            catch 
            {
            }
        }
        
        // Načtení a spárování studentů
        if (File.Exists(_cestaStudenti))
        {
            try
            {
                string json = File.ReadAllText(_cestaStudenti);
                var zaznamy = JsonSerializer.Deserialize<List<ZamestnanecZaznam>>(json) ?? [];
                foreach (var z in zaznamy)
                    studenti.Add(new Student(z.Jmeno, z.Prijmeni, z.Kod, z.PracovniPozice, z.Firma));
            }
            catch 
            {
            }
        }

        return (studenti);
    }

    public void Ulozit(List<Student> studenti)
    {
        // Auto-generování složek (All-in-One pojistka)        
        Directory.CreateDirectory(_slozkaStudenti);
        
        // Serializace studentů
        var zaznamyStudentu = new List<ZamestnanecZaznam>();
        foreach (var s in studenti)
            zaznamyStudentu.Add(new ZamestnanecZaznam(s.Jmeno, s.Prijmeni, s.Kod!, s.PracovniPozice, s.Firma, JeLektorem: false));

        string jsonStudenti = JsonSerializer.Serialize(zaznamyStudentu, JsonOptions);
        string tmpStudenti = _cestaStudenti + ".tmp";
        File.WriteAllText(tmpStudenti, jsonStudenti);
        File.Move(tmpStudenti, _cestaStudenti, overwrite: true);
        ZalohovaciSluzba.ProvedAutomatickouZalohu(_cestaStudenti);
    }
}