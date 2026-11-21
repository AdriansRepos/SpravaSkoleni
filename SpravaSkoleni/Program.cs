using SpravaSkoleni.Vypisy;
using SpravaSkoleni.Zaloha;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]
// Automatický tichá kontrola roční uzávěrky a čistky retenčních souborů na pozadí ještě před spuštěním aplikace
await ArchivacniSluzba.ZkontrolujRocniArchivaciAsync();
await HlavniMenuVypisy.SpustitAplikaciAsync();