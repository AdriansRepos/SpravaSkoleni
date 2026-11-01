using SpravaSkoleni.Logika;
using SpravaSkoleni.Menu;
using SpravaSkoleni.Zaloha;

// Automatický tichý check roční uzávěrky a čistky retenčních souborů na pozadí
await ArchivacniSluzba.ZkontrolujRocniArchivaciAsync();
await HlavniMenu.SpustitAplikaciAsync();