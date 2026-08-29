using SpravaSkoleni.Logika;
using SpravaSkoleni.Menu;
using SpravaSkoleni.Zaloha;

var archivace = new ArchivacniSluzba();
archivace.ZkontrolujRocniArchivaci();

SpravceSkoleni spravceSkoleni = new();
spravceSkoleni.NacistZamestnance();
spravceSkoleni.NacistSkoleni();

HlavniMenu.ZobrazVyberMenu(spravceSkoleni);