using SpravaSkoleni.Logika;
using SpravaSkoleni.Menu;

SpravceSkoleni spravceSkoleni = new();
spravceSkoleni.NacistZamestnance();
spravceSkoleni.NacistSkoleni();

HlavniMenu.ZobrazVyberMenu(spravceSkoleni);