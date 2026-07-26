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

- **Zamestnanec** – společný základ pro `Student` a `Lektor` (jméno, příjmení, kód, pracovní pozice)
- **Student** – dědí ze `Zamestnanec`, eviduje seznam školení, kterých se účastní
- **Lektor** – dědí ze `Zamestnanec`, eviduje seznam školení, která vyučuje
- **Skoleni** – název, popis, termín, doporučený počet účastníků, seznam přihlášených studentů a přiřazených lektorů, hodnocení studentů (u uzavřených školení), stav otevřeno/uzavřeno

---

## Ukládání dat

Data se ukládají do textových souborů:

- `zamestnanci.txt` – seznam všech zaměstnanců (lektorů a studentů) s příznakem role
- `skoleni.txt` – seznam všech školení včetně přiřazených lektorů, studentů a jejich hodnocení

Formát souboru `skoleni.txt` používá strukturovaný zápis s návěštími (`SKOLENI`, `LEKTORI`, `STUDENTI`, `HODNOCENI`), podle kterých se při načítání pozná, která část souboru se právě čte.

---

## Struktura projektu
/SpravaSkoleni

├── Program.cs

├── SpravceSkoleni.cs

├── Zamestnanec.cs

├── Student.cs

├── Lektor.cs

├── Skoleni.cs

└── README.md

---

## Použité techniky

- OOP (dědičnost – `Student` a `Lektor` dědí ze společné třídy `Zamestnanec`)
- Zapouzdření a validace vstupů
- Práce se soubory (`StreamReader`, `StreamWriter`)
- Strukturovaný textový formát s návěštími pro serializaci vztahů mezi objekty (školení ↔ lektoři ↔ studenti)
- Kolekce (`List<T>`, `Dictionary<TKey, TValue>`)
- Vícevrstvé konzolové menu (hlavní menu → detail školení → úpravy školení)
- Moderní syntaxe C# (collection expressions, ternární operátory) tam, kde nahrazuje původní učebnicový zápis

---

## Ukázka kódu

```csharp
public void PriraditLektora(Skoleni skoleni)
{
    // Sestavíme seznam lektorů, kteří ještě nejsou přiřazeni na toto školení.
    List<Lektor> lektori = [];
    foreach (Lektor lektor in Lektori)
    {
        if (!skoleni.Lektori.Contains(lektor))
        {
            lektori.Add(lektor);
        }
    }

    Console.WriteLine("*** Přiřazení lektora na školení ***");
    for (int i = 0; i < lektori.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {lektori[i].Jmeno} {lektori[i].Prijmeni} ({lektori[i].Kod})");
    }
    Console.WriteLine("0. Zpět");
    Console.Write("Přiřadit lektora: ");

    int volba = int.Parse(Console.ReadLine()!) - 1;
    if (volba >= 0 && volba < lektori.Count)
    {
        skoleni.Lektori.Add(lektori[volba]);
        UlozitSkoleni();
        Console.WriteLine("! Lektor byl úspěšně přiřazen na školení !");
    }
}
```

---

## Plánované funkce

- Rozvrstvení architektury na datovou, logickou a komunikační vrstvu (oddělení `Console.WriteLine`/`ReadLine` od business logiky)
- Přechod na JSON serializaci místo textového formátu s návěštími
- Crash-safe ukládání a automatická záloha dat (podle vzoru z projektu ZOO)
- Validace vstupů přes `TryParse` místo `Parse`
- Vyhledávání a filtrování školení podle data/lektora/stavu
- GUI verze (WPF)

---

## Release

**v1.0.0** – První funkční verze podle učebnicového základu: evidence školení, přihlašování/odhlašování studentů, přiřazování/odebírání lektorů, uzavírání školení, ukládání do textových souborů.  
[Stáhnout zde](https://github.com/AdriansRepos/SpravaSkoleni/releases/tag/v1.0.0)