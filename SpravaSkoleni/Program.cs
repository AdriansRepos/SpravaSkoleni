using SpravaSkoleni.Logika;
using SpravaSkoleni.Menu;
using SpravaSkoleni.Vypisy;
using SpravaSkoleni.Zaloha;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]
// Automatický tichý check roční uzávěrky a čistky retenčních souborů na pozadí
await ArchivacniSluzba.ZkontrolujRocniArchivaciAsync();
await HlavniMenuVypisy.SpustitAplikaciAsync();