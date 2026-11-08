## 🌐 Jazyk / Language Selection

*Klikněte na sekci níže pro zobrazení v češtině nebo angličtině.*
*Click on a section below to toggle between Czech and English.*

---

<details open>
<summary>🇨🇿 <strong>ČEŠTINA (Czech Version)</strong></summary>

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

### B2B Architektura & Multi-tenancy
Aplikace využívá koncept sdílené databáze s logickou izolací dat (Shared Database, Isolated Data). Veškerá školení jsou v JSON souborech označena příslušným identifikátorem firmy (Tenant). Systém při inicializaci `SpravceSkoleni` filtruje data na úrovni repozitářů, čímž je zajištěno, že uživatelé vidí pouze kurzy patřící jejich organizaci.

### Čistá architektura (Clean Architecture)
Projekt striktně dodržuje princip Single Responsibility Principle (SRP) a oddělení vrstev:
1. **Domain/Application Layer (`SpravceSkoleni`, `SpravceExportu`)**: Obsahuje čistou byznys logiku. Je zcela nezávislá na uživatelském rozhraní (UI) a konzoli. Veškerá selhání validací vyjadřuje pomocí výjimek (`Exceptions`). Tato vrstva je 100% pokrýtelná Unit testy.
2. **Data Layer (`Repozitare`)**: Zajišťuje perzistenci dat do JSON souborů a správu crash-safe záloh.
3. **Presentation Layer (`Menu`, `Vypisy`)**: Jediná vrstva komunikující s uživatelem. Odpovídá za zachytávání výjimek z logické vrstvy a jejich formátované zobrazení (pomocí `BarevneVypisyHelper`).

### Asynchronní zpracování & Multithreading (End-to-End Async)
Aplikace plně využívá moderní asynchronní vzory platformy .NET pro maximalizaci plynulosti uživatelského rozhraní:
1. **Asynchronní I/O řetězec**: Veškerá komunikace s pevným diskem (čtení/zápis JSON dat a textových exportů) probíhá neblokujícím způsobem přes `async/await`. Během fyzického zápisu na disk se procesorové vlákno vrací do fondu (Thread Pool) a je připraveno reagovat na jiné akce.
2. **Paralelní infrastruktura (`Task.WhenAll`)**: Roční systémová archivace dat spouští procesy pro jednotlivé soubory paralelně na pozadí, čímž maximalizuje využití vícejádrových procesorů při startu aplikace.
3. **Multithreaded Fire-and-Forget (`Task.Run`)**: Tiché automatické zálohy jsou kompletně izolované od hlavního vlákna. Systém po uložení dat pouze deleguje vytvoření zálohy na

### Hardwarové periferie, PDF a tisk
- **Integrace externí knihovny `UsbPeripheralsLib`:** Kompletní obalení a české lokalizované rozhraní pro správu vyměnitelných úložišť a tiskových výstupů.
- **Detekce USB a offline zálohy:** Automatická detekce připojených vyměnitelných USB disků (zobrazení jmenovky, volného a celkového místa) s možností přímého exportu záloh na externí paměti.
- **Generování PDF dokumentů:** Využití QuestPDF engine pro rychlé generování oficiálních potvrzení o absolvování školení do formátu PDF.
- **Systémový tisk:** Podpora tisku na lokální/kabelové tiskárny přes systémovou tiskovou frontu Windows.
- **Síťový RAW tisk:** Přímý tisk přes TCP sockets (port 9100) na síťové tiskárny bez nutnosti instalovaných ovladačů.

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
│       ├── LektoriRepository.cs
│       ├── SkoleniRepository.cs
│       └── StudentiRepository.cs
├── Enumy/
│   ├── ExportMenuVolba.cs
│   ├── HlavniMenuVolba.cs
│   ├── SkoleniDetailMenuVolba.cs
│   ├── SkoleniMenuHlavniVolba.cs
│   ├── UcastniciMenuVolba.cs
│   ├── UsbMenuVolba.cs
│   └── VyhledavaniMenuVolba.cs
├── Logika/
│   ├── SpravceExportu.cs
│   └── SpravceSkoleni.cs
├── Menu/
│   ├── ExportMenu.cs
│   ├── HlavniMenu.cs
│   ├── SkoleniMenu.cs
│   ├── UcastniciMenu.cs
│   ├── UsbPeripheralsMenu.cs
│   └── VyhledavaniMenu.cs
├── Vypisy/
│   ├── ExportMenuVypisy.cs
│   ├── HlavniMenuVypisy.cs
│   ├── LektoriVypisy.cs
│   ├── SkoleniMenuVypisy.cs
│   ├── SkoleniVypisy.cs
│   ├── StudentiVypisy.cs
│   ├── UcastniciMenuVypisy.cs
│   ├── UsbPeripheralsMenuVypisy.cs
│   └── VyhledavaniMenuVypisy.cs
├── Zaloha/
│   ├── ArchivacniSluzba.cs
│   └── ZalohovaciSluzba.cs
├── DiagramTridAplikace.cd
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

- **Subsystem Fúze (ZOO)** – kompletní integrace modulu Správy školení jako autonomního subsystému do hlavní enterprise aplikace ZOO (https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2). Stejně jako ponechání modulu správy školení jako samostatné plně funkční aplikace.
- **GUI (WPF)**

---

## Release

**v1.0.0** – První funkční verze podle učebnicového základu: evidence školení, přihlašování/odhlašování studentů, přiřazování/odebírání lektorů, uzavírání školení, ukládání do textových souborů.  

**v1.1.0** – Nahrazeno ruční číslované vypisování a parsování voleb (`for` cyklus + `int.Parse`) za znovupoužitelnou knihovnu `SelectHelper` napříč všemi metodami výběru (výběr školení, přihlášení/odhlášení studenta, přiřazení/odebrání lektora). Zadávání dat (počet účastníků, data konání, hodnocení studentů) nahrazeno validovaným vstupem přes `TextHelper`, s možností opakovaného zadání při chybě místo pádu aplikace. Třída `Zamestnanec` upravena na abstraktní bázi s automatickým převodem jména a příjmení do TitleCase přes `InputHelper`, zděděným do `Student` a `Lektor`. Opraveny logické chyby (obrácená podmínka při návratu ze seznamu školení, chybějící odečtení indexu při výběru).  

**v1.2.0** – Přechod z textového formátu s návěštími na JSON serializaci (`zamestnanci.json`, `skoleni.json`). Třída `Skoleni` ukládá jen kódy studentů/lektorů místo celých objektů, aby se předešlo kruhové referenci; skutečné vazby se dopojují po načtení. `SpravceSkoleni` rozdělena na partial soubory (`SpravceSkoleni.SkoleniData.cs`, `SpravceSkoleni.ZamestnanciData.cs`) pro přehlednost. Metody přejmenovány bez přípony "Json" (např. `UlozitSkoleniJson` → `UlozitSkoleni`), protože JSON je teď jediný podporovaný formát.  

**v1.2.1** – Refaktorování architektury datové vrstvy. Odstraněny partial soubory třídy SpravceSkoleni a nahrazeny samostatnými třídami repozitářů (ZamestnanciRepository, SkoleniRepository). Dosaženo čistého oddělení odpovědností (SRP) – SpravceSkoleni se stará o logiku a prezentaci, zatímco repozitáře čistě o persistenci do JSON souborů.

**v1.2.2** – Kompletní strukturální refaktorování architektury bez změny vnějších funkcí pro uživatele. Kód byl přehledně rozdělen do samostatných jmenných prostorů a složek (`Data/Entity`, `Data/Repozitare`, `Logika`, `Menu`, `Vypisy`). Aplikační logika ve `SpravceSkoleni` byla plně očistěna od konzolových výpisů (`Console.WriteLine`), které byly přesunuty do prezentace. Přidána integrace sdílené knihovny `BarevneVypisyHelper` pro unifikované stylování konzolového výstupu.  

**v1.3.0** – Nasazena pokročilá defenzivní logika: kontrola platnosti datumu, hlídání překrývání termínů a blokace přihlášení při naplnění kapacity.

**v1.4.0** – Masivní refaktorizace na unikátní podnikovou identifikaci (Kódy a Příjmení) eliminující shodu jmen. Nasazen vysoce optimalizovaný exportní modul prezenčních listin na disk s integrovanou byznysovou statistikou a nulovou paměťovou alokací (CA1862). Architektura byla rozšířena o pokročilé LINQ vyhledávací a filtrační subsystémy v samostatných souborech a kompletní evidenci historie účastníků s automatickým čištěním duplicit přes agregace. Prezentační vrstva byla modulárně rozvětvěna do izolovaných interaktivních podmenu (Vyhledávání, Účastníci) a kompletně aktualizován modelový diagram tříd (.cd). Aplikace je plně stabilizována a připravena pro přechod na vícevláknové zpracování a komunikaci s hardwarovými periferiemi.

**v1.5.0** – Architektura povýšena na B2B All-in-One standard rozdělením kmenových dat do tří samostatných složek (`Skoleni`, `Lektori`, `Studenti`). Implementováno autonomní crash-safe ukládání dat přes `.tmp` soubory, tiché automatické zálohování po každém zápisu disku a 5letá instanční roční archivace s defenzivním přeskočením. Odstraněn vývojářský balast v podobě ladicích výpisů z byznysové logiky i hlavního menu a plně optimalizován startovací proces aplikace v `Program.cs`.

**v1.6.0** – Zavedení B2B Multi-tenancy – Architektura aplikace byla rozšířena o podporu parametru Firma u zaměstnanců i školení, což umožňuje kompletní izolaci dat různých firemních zákazníků. Ochrana proti překlepům – Zadávání firmy je plně defenzivní a automaticky se formátuje přes TitleCase (např. dvi -> Dvi), což zajišťuje konzistenci dat. Podpora externích kurzů – Logika dopojování referencí, hodnocení a hlídání časových překryvů nově identifikuje studenty pomocí neprůstřelné dvojice Firma + Kód, zatímco lektoři zůstávají flexibilně interní. Doplnění Firmy studenta do prezenční listiny jako další kolonka. 
Dokončení striktního oddělení studentů od lektorů v namespace Vypisy, kde přišlo k vytvoření třídy StudentiVypisy, kam se přesunul kód pro výpisy studentů a přejmenování třídy ZamestnanciVypisy na LektoriVypisy. V této přejmenované třídě zůstal pouze kód pro výpisy lektorů.

**v1.6.1** – Čistá architektura (Clean Architecture)
Projekt striktně dodržuje princip Single Responsibility Principle (SRP) a oddělení vrstev:
1. **Domain/Application Layer (`SpravceSkoleni`, `SpravceExportu`)**: Obsahuje čistou byznys logiku. Je zcela nezávislá na uživatelském rozhraní (UI) a konzoli. Veškerá selhání validací vyjadřuje pomocí výjimek (`Exceptions`). Tato vrstva je 100% pokrýtelná Unit testy.
2. **Data Layer (`Repozitare`)**: Zajišťuje perzistenci dat do JSON souborů a správu crash-safe záloh.
3. **Presentation Layer (`Menu`, `Vypisy`)**: Jediná vrstva komunikující s uživatelem. Odpovídá za zachytávání výjimek z logické vrstvy a jejich formátované zobrazení (pomocí `BarevneVypisyHelper`).

Asynchronní zpracování & Multithreading (End-to-End Async)
Aplikace plně využívá moderní asynchronní vzory platformy .NET pro maximalizaci plynulosti uživatelského rozhraní:
1. **Asynchronní I/O řetězec**: Veškerá komunikace s pevným diskem (čtení/zápis JSON dat a textových exportů) probíhá neblokujícím způsobem přes `async/await`. Během fyzického zápisu na disk se procesorové vlákno vrací do fondu (Thread Pool) a je připraveno reagovat na jiné akce.
2. **Paralelní infrastruktura (`Task.WhenAll`)**: Roční systémová archivace dat spouští procesy pro jednotlivé soubory paralelně na pozadí, čímž maximalizuje využití vícejádrových procesorů při startu aplikace.
3. **Multithreaded Fire-and-Forget (`Task.Run`)**: Tiché automatické zálohy jsou kompletně izolované od hlavního vlákna. Systém po uložení dat pouze deleguje vytvoření zálohy na pozadí a okamžitě uživatele vrací do menu bez jakéhokoli čekání. 

**v1.7.0** – Modul rozhraní hardwarových periferií & oddělené Enumy
- Integrována externí knihovna `UsbPeripheralsLib` pro správu USB úložišť (detekce disků, offline zálohování), generování PDF certifikátů (`QuestPDF`) a dvoucestný tisk (kabelový přes Windows frontu i přímý RAW síťový tisk na portu 9100).
- Kompletní obalení anglické knihovny v konzolové prezentaci do českého rozhraní za dodržení striktního oddělení menu od výpisů.
- Vytvořena nová samostatná složka a namespace `Enumy`, kam byly vyčleněny veškeré řídící enumy voleb menu (`UsbMenuVolba`, `VyhledavaniMenuVolba`, `HlavniMenuVolba` a další).

[Aktuální verze ke stažení zde](https://github.com/AdriansRepos/SpravaSkoleni/releases)

</details>

---

<details>
<summary>🇺🇸 <strong>ENGLISH (English Version)</strong></summary>

# Training Management System (SpravaSkoleni)

A robust, production-ready enterprise console application built in **C# (.NET)** designed for comprehensive management of corporate trainings, instructors, and enrolled participants. Rooted in a solid educational foundation, it has been systematically evolved into a modern, multi-threaded B2B system featuring clean architecture, crash-safe data persistence, and strict data isolation.

---

## Features

### Trainings
- **Creation & Lifecycle:** Set up new trainings with custom titles, descriptions, recommended capacity limits, and start/end dates.
- **Status Management:** View open/closed training lists and detailed views including instructors, enrolled students, and feedback ratings for closed courses.
- **Defensive Client Logic:** Real-time date validation (preventing end dates prior to start dates) enforced via cyclical input prompts without crashing or resetting the entire form.

### Students & Instructors
- **Enrollment & Assignment:** Seamlessly enroll/unenroll students and assign/remove instructors for open trainings.
- **Strict Integrity Locks:** Zero modifications permitted on already closed/completed courses.
- **Time Slot Collision Guards:** Automatic schedule overlap protection—prevents instructors and students from attending overlapping courses simultaneously.
- **Capacity Hard-Stop:** Automated blocking of student registrations when recommended capacity reaches 100%.
- **Unique Enterprise Identification:** All error handling, lookups, and validations rely on a secure composite identifier (`Surname` + unique employee `Code`) instead of ambiguous first names.

### Advanced Search & Filtering
- **Interactive Submenus:** Isolated console submenus dedicated to advanced data queries.
- **Status & Full-Text Filtering:** Rapidly list trainings filtered strictly by status or perform case-insensitive full-text searches by title, instructor surname, or employee code.
- **Date-Range Overlap Queries:** Search for courses within specific date ranges using mathematical time-axis overlap calculations.
- **Seamless Navigation:** Smooth transitions from filtered search results directly into the specific training's detail view.

### Participant History & Management
- **Centralized Directory:** Unique overview of all system-registered students/instructors with automatic duplicate purging via LINQ aggregations (`GroupBy`).
- **Employee Timeline Cards:** Detailed chronological history displaying all open and closed courses for any specific employee.

### Attendance Sheet Export
- **File Export:** Generate formatted attendance lists into `.txt` files saved within an automatically managed `Exports` directory.
- **Rich Metadata:** Exports include course metadata, descriptions, capacity statistics, and a detailed student roster with physical signature lines.
- **Optimized I/O Performance:** Leverages `StringBuilder`, zero-allocation string comparisons (`StringComparison.OrdinalIgnoreCase`), and optimized collection checks (`.Count > 0` adhering to CA1860, CA1862).

### Data Security & Automation
- **B2B All-in-One Structure:** Physical data segregation into dedicated, automatically generated root directories (`Skoleni/`, `Lektori/`, `Studenti/`).
- **Crash-Safe Persistence:** Prevents data corruption during unexpected power outages or failures by using atomic writes via temporary `.tmp` files followed by safe system replacement (`File.Move` with overwrite).
- **Autonomous Silent Backups:** Automatic generation of `.bak.json` backup files into a `Backups/` directory after every successful write operation, entirely decoupled from user intervention.
- **5-Year Institutional Archiving:** Automated year-end closing checks at application startup. Freezes past-year data into an `Archive/` folder and purges records older than 5 years with defensive skip-locks.

### B2B Multi-Tenancy
The application implements a **Shared Database, Isolated Data** pattern. All trainings and employee records are tagged with a corporate tenant identifier (`Firma`). During initialization, `SpravceSkoleni` filters data at the repository layer, ensuring users access exclusively their organization's courses.

### Clean Architecture
Strictly adheres to the Single Responsibility Principle (SRP) and separation of concerns:
1. **Domain/Application Layer (`SpravceSkoleni`, `SpravceExportu`)**: Contains pure business logic. Completely independent of the user interface (UI) and console. All validation failures are expressed through custom exceptions. Fully unit-testable.
2. **Data Layer (`Repozitories`)**: Handles JSON persistence and crash-safe backup operations.
3. **Presentation Layer (`Menu`, `Vypisy`)**: The sole user-facing layer responsible for catching domain exceptions and rendering formatted outputs via `BarevneVypisyHelper`.

### End-to-End Asynchronous Processing & Multithreading
- **Asynchronous I/O Chain:** All disk communications (JSON read/write operations and text exports) execute non-blockingly via `async/await`, returning thread pool threads during physical disk I/O.
- **Parallel Infrastructure (`Task.WhenAll`)**: Yearly system archiving executes parallel file processing in the background, maximizing multi-core CPU utilization at startup.
- **Multithreaded Fire-and-Forget (`Task.Run`)**: Silent background backups are completely isolated from the main thread, instantly returning control to the user without UI blocking.

### Hardware Peripherals, PDF & Printing
- **`UsbPeripheralsLib` Integration:** Full wrapping and localized Czech UI overlay for removable storage and print hardware management.
- **USB Detection & Offline Backups:** Automated detection of connected removable USB storage devices (displaying volume label, available, and total space) with direct backup export functionality.
- **PDF Document Generation:** Integrated QuestPDF engine for fast PDF creation of training completion certificates.
- **System Printing Queue:** Support for local/wired Windows system print queue jobs.
- **Network RAW Printing:** Direct TCP socket communication (port 9100) for network printers bypassing OS print driver dependencies.

---

## Data Model

- **`Zamestnanec` (Employee):** Abstract base class for `Student` and `Lektor`, storing first/last names (automatically formatted to TitleCase), employee code, and job position.
- **`Student`:** Inherits from `Zamestnanec`, tracks enrolled courses.
- **`Lektor` (Instructor):** Inherits from `Zamestnanec`, tracks taught courses.
- **`Skoleni` (Training):** Stores title, description, schedule, capacity limits, enrolled student codes, assigned instructor codes, student ratings (for closed courses), and open/closed status. Only codes (not full object graphs) are serialized to JSON to prevent circular references; actual object references are dynamically wired up upon loading.

---

## External Shared Libraries

- **TextHelper:** Robust user input processing and validation (`UpravaVstupu.ZeptejSeAUprav`).
- **SelectHelper:** Interactive list item selection UI component (`SelectHelp.VybratPolozku`).
- **InputHelper:** Automatic TitleCase text conversion (`TitleCase.ToTitleCase`).
- **BarevneVypisyHelper:** Unified console styling and colored output formatting (`VypisyDoKonzole`).

These libraries are shared with [EvidenceZOOCviceniUpraveno2](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2).

---

## Project Structure

```text
/SpravaSkoleni
├── Data/
│   ├── Entity/
│   │   ├── Lektor.cs
│   │   ├── Skoleni.cs
│   │   ├── Student.cs
│   │   └── Zamestnanec.cs
│   └── Repozitare/
│       ├── LektoriRepository.cs
│       ├── SkoleniRepository.cs
│       └── StudentiRepository.cs
├── Enumy/
│   ├── ExportMenuVolba.cs
│   ├── HlavniMenuVolba.cs
│   ├── SkoleniDetailMenuVolba.cs
│   ├── SkoleniMenuHlavniVolba.cs
│   ├── UcastniciMenuVolba.cs
│   ├── UsbMenuVolba.cs
│   └── VyhledavaniMenuVolba.cs
├── Logika/
│   ├── SpravceExportu.cs
│   └── SpravceSkoleni.cs
├── Menu/
│   ├── ExportMenu.cs
│   ├── HlavniMenu.cs
│   ├── SkoleniMenu.cs
│   ├── UcastniciMenu.cs
│   ├── UsbPeripheralsMenu.cs
│   └── VyhledavaniMenu.cs
├── Vypisy/
│   ├── ExportMenuVypisy.cs
│   ├── HlavniMenuVypisy.cs
│   ├── LektoriVypisy.cs
│   ├── SkoleniMenuVypisy.cs
│   ├── SkoleniVypisy.cs
│   ├── StudentiVypisy.cs
│   ├── UcastniciMenuVypisy.cs
│   ├── UsbPeripheralsMenuVypisy.cs
│   └── VyhledavaniMenuVypisy.cs
├── Zaloha/
│   ├── ArchivacniSluzba.cs
│   └── ZalohovaciSluzba.cs
├── DiagramTridAplikace.cd
├── Program.cs
└── README.md
```

---

## Core Techniques & Patterns

- OOP Principles: Inheritance (Student & Lektor inherit from abstract Zamestnanec), encapsulation.
- Multi-Layered Architecture: Strict separation of UI, Business Logic, and Data Persistence.
- Repository Pattern: Clean abstraction layer for JSON file persistence.
- Modern JSON Serialization (System.Text.Json): Utilizes [JsonIgnore] to prevent circular reference exceptions on bidirectional relationships.
- LINQ Queries: Extensive use of Select, Where, GroupBy, and ToDictionary for data binding and aggregation.
- Modern C# Syntax: Collection expressions, target-typed new, nullable reference types, records, pattern matching.

---

## Code Sample

```csharp
// Example of clean domain logic in SpravceSkoleni decoupled from console UI presentation
public void AssignInstructor(Skoleni training, Lektor instructor)
{
    if (!training.Lektori.Contains(instructor))
    {
        training.Lektori.Add(instructor);
        SaveTraining();
    }
}

public List<Lektor> GetUnassignedInstructors(Skoleni training)
{
    return Lektori.Where(instructor => !training.Lektori.Contains(instructor)).ToList();
}
```

---

## Planned Features

- ZOO Fusion Subsystem: Complete integration of the Training Management module as an autonomous subsystem into the primary enterprise ZOO application (EvidenceZOOCviceniUpraveno2), while maintaining its standalone execution capability.
- Graphical User Interface (WPF)

---

## Release History

**v1.0.0** – Initial educational release: basic training records, student enrollments, instructor assignments, training closure, and text file persistence.

**v1.1.0** – Replaced manual index-based loops with the reusable SelectHelper library. Integrated validated inputs via TextHelper. Converted Zamestnanec to an abstract base with automated TitleCase formatting.

**v1.2.0** – Transitioned from custom text formats to JSON serialization (System.Text.Json). Split SpravceSkoleni into partial files for clarity.

**v1.2.1** – Architectural refactoring: introduced dedicated repository classes (ZamestnanciRepository, SkoleniRepository) adhering strictly to SRP.

**v1.2.2** – Complete structural refactoring into clean namespaces and folders. Purged Console.WriteLine statements from business logic, migrating them to presentation classes powered by BarevneVypisyHelper.

**v1.3.0** – Deployed advanced defensive logic: date validations, schedule overlap protection, and capacity hard-stops.

**v1.4.0** – Massive refactoring introducing unique enterprise identification (Codes & Surnames). Added zero-allocation attendance export module and advanced LINQ search/filtering submenus.

**v1.5.0** – Elevated architecture to B2B All-in-One standard with segmented root folders, crash-safe .tmp atomic writes, silent background backups, and 5-year yearly archiving.

**v1.6.0** – Introduced B2B Multi-tenancy (Firma tenant isolation), strict namespace separation (StudentiVypisy vs LektoriVypisy), and robust tenant-aware reference binding.

**v1.6.1** – Finalized Clean Architecture boundaries and implemented End-to-End Asynchronous processing (async/await, Task.WhenAll parallel archiving, and Task.Run fire-and-forget backups).

**v1.7.0** – Hardware Peripherals Integration & Enum Layer Refactoring
- Integrated external `UsbPeripheralsLib` library for USB storage management (drive detection, offline backups), PDF document creation via `QuestPDF`, and dual-mode printing (local Windows queue and direct network RAW sockets on port 9100).
- Fully wrapped library English API calls inside a localized Czech console interface maintaining strict Clean Architecture principles (Menu vs Vypisy UI layer separation).
- Introduced a dedicated `Enumy` directory and namespace housing all menu control enums (`UsbMenuVolba`, `VyhledavaniMenuVolba`, `HlavniMenuVolba`, etc.).

[Downolad latest release here](https://github.com/AdriansRepos/SpravaSkoleni/releases)

</details>