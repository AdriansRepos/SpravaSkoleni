# Správa školení – Konzolová aplikace v C#

Tento projekt je konzolová aplikace pro evidenci firemních školení, jejich lektorů a přihlášených studentů. Vychází z učebnicového základu, který dál rozšiřuji a upravuji vlastním způsobem (moderní syntaxe C#, opravy chyb z originálu, plánované rozvrstvení architektury).

---

## Funkce aplikace

### Školení

- Vytvoření nového školení (název, popis, doporučený počet účastníků, počáteční a koncové datum)
- Výpis seznamu všech školení se stavem (otevřené/uzavřené)
- Zobrazení detailu školení (lektoři, přihlášení studenti, u uzavřených školení i jejich hodnocení)
- Uzavření školení

### Studenti a lektoři

- Přihlášení studenta na otevřené školení
- Odhlášení studenta ze školení
- Přiřazení lektora na otevřené školení
- Odebrání lektora ze školení
- U uzavřených školení nelze provádět žádné úpravy (ochrana proti změně již proběhlého školení)

### Ladicí výpis

- Kompletní přehled všech zaměstnanců (lektorů, studentů) a školení pro účely vývoje a kontroly dat

---

## Datový model

- **Zamestnanec** – abstraktní společný základ pro `Student` a `Lektor` (jméno a příjmení s automatickým převodem do TitleCase, kód, pracovní pozice)

- **Student** – dědí ze `Zamestnanec`, eviduje seznam školení, kterých se účastní

- **Lektor** – dědí ze `Zamestnanec`, eviduje seznam školení, která vyučuje

- **Skoleni** – název, popis, termín, doporučený počet účastníků, seznam přihlášených studentů a přiřazených lektorů, hodnocení studentů (u uzavřených školení), stav otevřeno/uzavřeno. Do JSON se ukládají jen kódy studentů/lektorů, ne celé objekty; skutečné reference se dopojují po načtení.

---

## Ukládání dat

Data se ukládají do JSON souborů:

- `zamestnanci.json` – seznam všech lektorů a studentů s příznakem role
- `skoleni.json` – seznam všech školení

Aby se předešlo kruhové referenci při serializaci (`Student` obsahuje seznam `Skoleni`, které obsahuje seznam `Student`), školení si neukládá přímo objekty studentů a lektorů, ale jen jejich kódy (`KodyStudentu`, `KodyLektoru`, `Hodnoceni`). Skutečné objekty se po načtení dopojí podle kódu z již načtených zaměstnanců.

---

## Externí knihovny (samostatné projekty)

- **TextHelper** – zpracování a validace uživatelských vstupů (`UpravaVstupu.ZeptejSeAUprav`)
- **SelectHelper** – výběr položky ze seznamu (`SelectHelp.VybratPolozku`)
- **InputHelper** – převod textu na TitleCase (`TitleCase.ToTitleCase`)
- **BarevneVypisyHelper** – formátování a barevný výstup menu i systémových hlášení do konzole (`VypisyDoKonzole`)

Tyto knihovny jsou sdílené i s projektem [EvidenceZOOCviceniUpraveno2](https://github.com/AdriansRepos/EvidenceZOOCviceniUpraveno2), kde vznikly a dál se rozvíjejí.

---

## Struktura projektu
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
│       ├── SkoleniRepository.cs
│       └── ZamestnanciRepository.cs
├── Logika/
│   └── SpravceSkoleni.cs
├── Menu/
│   └── HlavniMenu.cs
├── Vypisy/
│   └── SkoleniVypisy.cs
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

- **B2B datový model & Multi-tenancy** – podpora atributu `Firma` u entit i školení pro plné pokrytí externích zakázek, samoobslužného sdíleného přístupu i interního nenasazení (např. Temelín/DVI scénář)
- **Rozdělení datových repozitářů** – odříznutí kmenových zaměstnanců zákaznických firem od lektorů do samostatných souborů (`studenti.json` a `lektori.json`)
- **Export Prezenční listiny** – generování formátovaného textového výstupu prezenčky s kolonkami pro podpisy a rezervními řádky pro operativně dopsané náhradníky
- **Filtrování a vyhledávání školení** – přehledy podle stavu (otevřená/uzavřená) a vyhledávání podle názvu či termínu
- **Validace a defenzivní logika** – kontrola konzistence dat (koncové datum po počátečním) a vizuální upozornění při překročení doporučené kapacity školení
- **Crash-safe ukládání a automatická záloha dat** – zamezení poškození datových souborů při nečekaném ukončení aplikace a možnost vytváření časovaných záloh
- GUI (WPF)

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