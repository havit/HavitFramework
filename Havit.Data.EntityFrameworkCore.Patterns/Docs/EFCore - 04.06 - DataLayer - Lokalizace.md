## Lokalizace

Pokud máme model s lokalizacemi, pak službou `ILocalizationService` získáváme pro entitu z kolekce `Localizations` hodnotu pro zvolený jazyk.
Hodnotu můžeme získat pro “aktuální” nebo zvolený jazyk.

```csharp
ContryLocalization countryLocalization = localizationService.GetCurrentLocalization(country);
ContryLocalization countryLocalization = localizationService.GetLocalization(country, czechLanguage);
```

> ℹ️ Služba nezajišťuje načtení kolekce `Localizations` z databáze, zajišťuje jen výběr požadované hodnoty z této kolekce.

Logika hledání pro daný jazyk je postavena takto:

* Pokud existuje položka pro zadaný jazyk, je použita tato.
* Není-li nalezena, zkouší se hledat pro jazyk dle "[neutrálnější culture](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=net-5.0)" jazyka.
* Není-li nalezena, zkouší se hledat pro jazyk dle invariantní culture (prázdný `UICulture`).

Například pro uživatele pracující v češtině se hledá položka pro jazyky dle `UICulture` postupně pro "cs-cz", "cs", "".

### Registrace DI
Použití služby je podmíněno registrací do DI containeru, což můžeme udělat extension metodou `AddLocalizationServices`.

```csharp
services
  ...
  .AddLocalizationServices<Language>()
  ...;
```
