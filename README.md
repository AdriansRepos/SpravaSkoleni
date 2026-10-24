# Správa školení – Konzolová aplikace v C#

Tento projekt je konzolová aplikace pro evidenci firemních školení, jejich lektorů a přihlášených studentů. Vychází z učebnicového základu, který dál rozšiřuji a upravuji vlastním způsobem (moderní syntaxe C#, opravy chyb z originálu, plánované rozvrstvení architektury).

---

## Funkce aplikace

### Školení

- Vytvoření nového školení (název, popis, doporučený počet účastníků, počáteční a koncové datum).
- Výpis seznamu všech školení se stavem (otevřené/uzavřené).
- Zobrazení detailu školení (lektoři, přihlášení studenti, u uzavřených školení i jejich hodnocení).
- Uzavření školení lektorem na konci výuky.
- **Defenzivní klientská logika:** Kontrola platnosti datumu (konec nemůže být před začátkem) vynucená cyklickým opakováním vstupu bez nutnosti přepisovat celý formulář.

### Studenti a lektoři

- Přihlášení studenta na otevřené školení.
- Odhlášení studenta ze školení.
- Přiřazení lektora na otevřené školení.
- Odebrání lektora ze školení.
- **Neprůstřelná integrita:** U uzavřených školení nelze provádět žádné úpravy (ochrana proti změně již proběhlého školení).
- **Časové pojistky:** Automatické hlídání překrývání termínů – lektor nemůže učit a student nemůže studovat více kurzů v totožný čas.
- **Kapacitní stopka:** Tvrdá blokace přihlášení studenta při stoprocentním naplnění doporučené kapacity školení.
- **Unikátní podniková identifikace:** Veškeré chybové hlášky a vyhledávání nově využívají bezpečnou kombinaci `Prijmeni` a unikátního zaměstnaneckého `Kod` namísto křestních jmen.

### Vyhledávání a filtrace

- Izolované interaktivní konzolové podmenu pro pokročilé datové dotazy.
- Rychlý výpis školení filtrovaný striktně podle stavu (pouze otevřená / pouze uzavřená).
- Full-textové vyhledávání školení podle názvu, příjmení lektora nebo unikátního kódu zaměstnance s ignorováním velikosti písmen (case-insensitive).
- Vyhledávání v konkrétním rozmezí datumů konání pomocí matematického překryvu časových os.
- Plynulý přechod z filtrovaného výběru přímo do detailu vybraného školení.

### Správa účastníků a historie

- Samostatné klientské podmenu pro kompletní přehled firemního vzdělávání.
- Unikátní přehled všech studentů/lektorů registrovaných v systému s automatickým čištěním duplicit přes LINQ agregace (`GroupBy`).
- Kompletní časová karta zaměstnance zobrazující detailní historii všech jeho otevřených i uzavřených kurzů.

### Export prezenčních listin

- Export formátované prezenční listiny do souboru `.txt` do automaticky generované složky `Exporty`.
- Soubor obsahuje kompletní metadata kurzu, popis, statistiky kapacity a detailní seznam studentů s kolonkou pro fyzický podpis.
- **Optimalizovaný I/O provoz:** Využití StringBuilderu, nulová paměťová alokace nových řetězců (`StringComparison.OrdinalIgnoreCase`) a eliminace rozšiřujících LINQ metod ve prospěch vlastnosti `.Count > 0` (CA1860, CA1862).

### Datová bezpečnost a automatizace

- **B2B All-in-One struktura:** Fyzické odříznutí datových repozitářů do samostatných, automaticky vytvářených složek na disku (`Skoleni/`, `Lektori/`, `Studenti/`).
- **Crash-safe ukládání:** Zamezení poškození dat při nečekaném výpadku proudu pomocí atomického zápisu přes dočasné `.tmp` soubory a systémové nahrazení ostrých dat (`File.Move` s overwrite).
- **Autonomní tichá záloha:** Automatické generování záložních souborů `.bak.json` do složky `Zalohy/` po každém úspěšném uložení, zcela odstíněné od zásahu uživatele.
- **5letá instanční archivace:** Automatický check roční uzávěrky při startu aplikace. Zmražení loňských dat do složky `Archiv/` a automatické promazávání souborů starších než 5 let s využitím defenzivního přeskočení (skip) při systémovém zámku.

---

## Datový model

- **Zamestnanec** – abstraktní společný základ pro `Student` a `Lektor` (jméno a příjmení s automatickým převodem do TitleCase, kód, pracovní pozice)

- **Student** – dědí ze `Zamestnanec`, eviduje seznam školení, kterých se účastní

- **Lektor** – dědí ze `Zamestnanec`, eviduje seznam školení, která vyučuje

- **Skoleni** – název, popis, termín, doporučený počet účastníků, seznam přihlášených studentů a přiřazených lektorů, hodnocení studentů (u uzavřených školení), stav otevřeno/uzavřeno. Do JSON se ukládají jen kódy studentů/lektorů, ne celé objekty; skutečné reference se dopojují po načtení.

---

## Externí knihovny (samostatné projekty)

- **TextHelper** – zpracování a validace uživatelských vstupů (`UpravaVstupu.ZeptejSeAUprav`)
- **SelectHelper** – výběr položky ze seznamu (`SelectHelp.VybratPolozku`)
- **InputHelper** – převod textu na TitleCase (`TitleCase.ToTitleCase`)
- **BarevneVypisyHelper** – formátování a barevný výstup menu i systémových hlášení do konzole (`VypisyDoKonzole`)

Tyto knihovny jsou sdílené i s projektem [EvidenceZOOCviceniUpraveno2](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2), kde vznikly a dál se rozvíjejí.

---

## Struktura projektu

```text
/SpravaSkoleni
├── Data/
│   ├── Entity/
│   │   ├── Lektor.cs
│   │   ├── Skoleni.cs
│   │   ├── Student.cs
│   │   └── Zamestnanec.cs
│   └── Repozitare/
|       ├── SkoleniRepository.cs
|       ├── LektoriRepository.cs
│       └── StudentiRepository.cs
├── Logika/
│   ├── SpravceSkoleni.cs
│   └── SpravceExportu.cs
├── Menu/
│   └── HlavniMenu.cs
├── Vypisy/
│   ├── ZamestnanciVypisy.cs
│   └── SkoleniVypisy.cs
├── Zaloha/
│   ├── ArchivacniSluzba.cs
│   └── ZalohovaciSluzba.cs
├── Program.cs
└── README.md
```

---

## Použité techniky

- OOP (dědičnost – Student a Lektor dědí ze společné abstraktní třídy Zamestnanec)
- Zapouzdření a validace vstupů
- Vícevrstvá architektura s přísným oddělením odpovědností (UI / Vypisy / Menu → Logika → Data/Entity & Repozitáře)
- Vzor Repository pro práci s JSON soubory
- Znovupoužitelné externí knihovny (TextHelper, SelectHelper, InputHelper, BarevneVypisyHelper)
- Serializace/deserializace do JSON souborů (System.Text.Json)
- [JsonIgnore] pro zamezení kruhové reference při serializaci vztahu školení ↔ studenti/lektoři
- LINQ (Select, Where, ToDictionary) pro dopojení objektů po načtení podle uložených kódů
- Kolekce (List<T>, Dictionary<TKey, TValue>)
- Vícevrstvé konzolové menu (hlavní menu → detail školení → úpravy školení)
- Moderní syntaxe C# (collection expressions, ternární operátory, nullable reference types, records)
---

## Ukázka kódu

```csharp
// Ukázka čisté aplikační logiky ve SpravceSkoleni bez přímé závislosti na konzolové prezentaci
public void PriradLektora(Skoleni skoleni, Lektor lektor)
{
    if (!skoleni.Lektori.Contains(lektor))
    {
        skoleni.Lektori.Add(lektor);
        UlozitSkoleni();
    }
}

public List<Lektor> ZiskejNeprirazeneLektory(Skoleni skoleni)
{
    return Lektori.Where(lektor => !skoleni.Lektori.Contains(lektor)).ToList();
}
```

---

## Plánované funkce

- **B2B datový model & Multi-tenancy** – podpora atributu `Firma` u entit i školení pro plné pokrytí externích zakázek, samoobslužného sdíleného přístupu i interního nasazení (např. Temelín/DVI scénář).
- **Vícevláknová architektura a hardwarová integrace** – implementace asynchronního zpracování dat (multithreading) a zajištění přímé zabezpečené komunikace s externími periferiemi (tiskárny prezenčních listin, USB zálohovací disky).
- **Subsystem Fúze (ZOO)** – kompletní integrace modulu Správy školení jako autonomního subsystému do hlavní enterprise aplikace ZOO (https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2). Stejně jako ponechání modulu správy školení jako samostatné plně funkční aplikace.
- **GUI (WPF)**

---

## Release

**v1.0.0** – První funkční verze podle učebnicového základu: evidence školení, přihlašování/odhlašování studentů, přiřazování/odebírání lektorů, uzavírání školení, ukládání do textových souborů.  
[Stáhnout zde](https://github.com/AdriansRepos/SpravaSkoleni/releases/tag/v1.0.0)

**v1.1.0** – Nahrazeno ruční číslované vypisování a parsování voleb (`for` cyklus + `int.Parse`) za znovupoužitelnou knihovnu `SelectHelper` napříč všemi metodami výběru (výběr školení, přihlášení/odhlášení studenta, přiřazení/odebrání lektora). Zadávání dat (počet účastníků, data konání, hodnocení studentů) nahrazeno validovaným vstupem přes `TextHelper`, s možností opakovaného zadání při chybě místo pádu aplikace. Třída `Zamestnanec` upravena na abstraktní bázi s automatickým převodem jména a příjmení do TitleCase přes `InputHelper`, zděděným do `Student` a `Lektor`. Opraveny logické chyby (obrácená podmínka při návratu ze seznamu školení, chybějící odečtení indexu při výběru).  
[Stáhnout zde](https://github.com/AdriansRepos/SpravaSkoleni/releases/tag/v1.1.0)

**v1.2.0** – Přechod z textového formátu s návěštími na JSON serializaci (`zamestnanci.json`, `skoleni.json`). Třída `Skoleni` ukládá jen kódy studentů/lektorů místo celých objektů, aby se předešlo kruhové referenci; skutečné vazby se dopojují po načtení. `SpravceSkoleni` rozdělena na partial soubory (`SpravceSkoleni.SkoleniData.cs`, `SpravceSkoleni.ZamestnanciData.cs`) pro přehlednost. Metody přejmenovány bez přípony "Json" (např. `UlozitSkoleniJson` → `UlozitSkoleni`), protože JSON je teď jediný podporovaný formát.  
[Stáhnout zde](https://github.com/AdriansRepos/SpravaSkoleni/releases/tag/v1.2.0)

**v1.2.1** – Refaktorování architektury datové vrstvy. Odstraněny partial soubory třídy SpravceSkoleni a nahrazeny samostatnými třídami repozitářů (ZamestnanciRepository, SkoleniRepository). Dosaženo čistého oddělení odpovědností (SRP) – SpravceSkoleni se stará o logiku a prezentaci, zatímco repozitáře čistě o persistenci do JSON souborů.
[Stáhnout zde](https://github.com/AdriansRepos/SpravaSkoleni/releases/tag/v1.2.1)

**v1.2.2** – Kompletní strukturální refaktorování architektury bez změny vnějších funkcí pro uživatele. Kód byl přehledně rozdělen do samostatných jmenných prostorů a složek (`Data/Entity`, `Data/Repozitare`, `Logika`, `Menu`, `Vypisy`). Aplikační logika ve `SpravceSkoleni` byla plně očistěna od konzolových výpisů (`Console.WriteLine`), které byly přesunuty do prezentace. Přidána integrace sdílené knihovny `BarevneVypisyHelper` pro unifikované stylování konzolového výstupu.  
[Stáhnout zde](https://github.com/AdriansRepos/SpravaSkoleni/releases/tag/v1.2.2)

**v1.3.0** – Nasazena pokročilá defenzivní logika: kontrola platnosti datumu, hlídání překrývání termínů a blokace přihlášení při naplnění kapacity.
[Stáhnout zde](https://github.com/AdriansRepos/SpravaSkoleni/releases/tag/v1.3.0)

**v1.4.0** – Masivní refaktorizace na unikátní podnikovou identifikaci (Kódy a Příjmení) eliminující shodu jmen. Nasazen vysoce optimalizovaný exportní modul prezenčních listin na disk s integrovanou byznysovou statistikou a nulovou paměťovou alokací (CA1862). Architektura byla rozšířena o pokročilé LINQ vyhledávací a filtrační subsystémy v samostatných souborech a kompletní evidenci historie účastníků s automatickým čištěním duplicit přes agregace. Prezentační vrstva byla modulárně rozvětvěna do izolovaných interaktivních podmenu (Vyhledávání, Účastníci) a kompletně aktualizován modelový diagram tříd (.cd). Aplikace je plně stabilizována a připravena pro přechod na vícevláknové zpracování a komunikaci s hardwarovými periferiemi.
[Stáhnout zde](https://github.com/AdriansRepos/SpravaSkoleni/releases/tag/v1.4.0)

**v1.5.0** – Architektura povýšena na B2B All-in-One standard rozdělením kmenových dat do tří samostatných složek (`Skoleni`, `Lektori`, `Studenti`). Implementováno autonomní crash-safe ukládání dat přes `.tmp` soubory, tiché automatické zálohování po každém zápisu disku a 5letá instanční roční archivace s defenzivním přeskočením. Odstraněn vývojářský balast v podobě ladicích výpisů z byznysové logiky i hlavního menu a plně optimalizován startovací proces aplikace v `Program.cs`.
[Stáhnout zde](https://github.com/AdriansRepos/SpravaSkoleni/releases/tag/v1.5.0)