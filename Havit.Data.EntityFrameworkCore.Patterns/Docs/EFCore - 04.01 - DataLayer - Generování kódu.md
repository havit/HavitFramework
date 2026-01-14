## Generátor kódu

Implementace tříd popsaných v následujících kapitolách je automaticky generována s umožněním vlastního rozšíření vygenerovaného kódu.

Kód je generován pomocí [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) `Havit.Data.EntityFrameworkCore.CodeGenerator.Tool` (musí být nainstalován), jenž spouští code generátor z NuGet balíčku `Havit.Data.EntityFrameworkCore.CodeGenerator`, který je zamýšlen pro použití v projektu `Entity`.

### Aktualizace dotnet toolu Havit.Data.EntityFrameworkCore.CodeGenerator.Tool

Aktualizace `Havit.Data.EntityFrameworkCore.CodeGenerator.Tool` se očekávají příležitostně, např. když se změní podporovaná verze `.NET`, atp. Změny (aktualizace) `Havit.Data.EntityFrameworkCore.CodeGenerator` jsou podle změn, které potřebujeme udělat do code generátoru.

> ℹ️ Tím, že `Havit.Data.EntityFrameworkCore.CodeGenerator.Tool` není "běžným nuget" balíčkem v projektu, ale jde o [dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools), nezobrazují se jeho aktualizace v Package Manageru.
> Pro aktualizaci je třeba z příkazové řádky spustit (aktuální složka ve složce se solution):
>
> ```
> dotnet tool update Havit.Data.EntityFrameworkCore.CodeGenerator.Tool
> ```

### Spuštění generátoru

Generátor lze spustit powershell skriptem `Run-CodeGenerator.ps1` v rootu projektu `DataLayer`.

Pro spuštění přímo z Visual Studia si musíme otevřít jakoukoliv konzoli (Terminal, Developer Powershell, Nuget Package Console), přepnout se do složky `DataLayer` a spustit.

Běh generátoru je relativně rychlý, generátor je obvykle hotov během pár sekund.

### Princip generátoru (aneb kde bere generátor data)

Generátor získává data z modelu `DbContextu` ([DbContext.Model](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext.model?view=efcore-2.1)). `DbContext` se hledá v assembly projektu `Entity`, získává se přes [DbContextActivator](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.design.dbcontextactivator?view=efcore-2.1), čímž získáme instanci přes [IDesignTimeDbContextFactory](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.design.idesigntimedbcontextfactory-1?view=efcore-2.1), pokud existuje.

Assembly pro `Entity` se hledá ve složce `Entity/bin` (a všech podsložkách), bere se poslední v čase, tj. nejaktuálnější. Tím řešíme případnou existenci více verzí assembly v případě existence lokálního buildu v `Debug` i v `Release` konfiguraci.

### Konfigurace generátoru kódu

V rootu projektu s generátorem kódu nebo ve složce se solution je možné mít soubor `efcore.codegenerator.json` s nastavením:

```json
{
	"ModelProjectPath": "...",
	"MetadataProjectPath": "...",
	"DataLayerProjectPath" : "...",
	"MetadataNamespace": "..."
}
```

* `ModelProjectPath` - název složky (musí obsahovat \*.csproj) nebo cesta k csproj, kam budou generována metadata z modelu, cesta je relativní vůči složce se solution. Výchozí hodnotou je Model\\Model.csproj. 
* `MetadataProjectPath` - název složky (musí obsahovat \*.csproj) nebo cesta k csproj, kam budou generována metadata z modelu, cesta je relativní vůči složce se solution. Výchozí hodnotou je Model\\Model.csproj (by default stejné ako `ModelProjectPath`).
* `DataLayerProjectPath` - název složky (musí obsahovat \*.csproj) nebo cesta k csproj, kam bude generován DataLayer kód, cesta je relativní vůči složce se solution. Výchozí hodnotou je DataLayer\\DataLayer.csproj.
* `MetadataNamespace`  namespace, do kterého se metadata generují, je možno použít strukturovaný namespace, např. `My.Customized.Metadata` (na disku budou metadata vygenerována do složky \_generated\\My\\Customized\\Metadata).
* `SuppressRemovingRelicRepositories` - umožní potlačit mazání repositories, které již nejsou v modelu (např. byly odstraněny). Výchozí hodnota je false (mazání probíhá).

### Co generuje

Viz dále uvedené:

* DbDataSource, FakeDataSource
* DbRepository
* DataEntries
* Metadata
* DataLayerServiceExtensions
    
### Generované soubory

V `DataLayer`u jsou generovány soubory:

* _generated\\DataEntries\\*Namespace*\\I*Entity*Entries.cs
* _generated\\DataEntries\\*Namespace*\\*Entity*Entries.cs
* _generated\\DataSources\\*Namespace*\\I*Entity*DataSource.cs
* _generated\\DataSources\\*Namespace*\\*EntityDb*DataSource.cs
* _generated\\DataSources\\*Namespace*\\Fakes\\Fake*Entity*DataSource.cs
* _generated\\Repositories\\*Namespace*\\I*Entity*Repository.cs
* _generated\\Repositories\\*Namespace*\\*Entity*DbRepository.cs
* _generated\\Repositories\\*Namespace*\\*Entity*DbRepositoryBase.cs
* _generated\\Repositories\\*Namespace*\\*Entity*DbRepositoryQueryProvider.cs
* _generated\\DataLayerServiceExtensions.cs

a dále jsou jednorázově vytvořeny soubory (tj. při opakovaném spuštění generátoru se nepřepisují, neaktualizují):

* Repositories\\*Namespace*\\I*Entity*Repository.cs
* Repositories\\*Namespace*\\*Entity*DbRepository.cs
    

V `Model`u (resp. dle nastavení `MetadataProjectPath`) jsou generovány soubory:
* _generated\\Metadata\\*Namespace*\\*Entity*Metadata.cs
    

### Poznámka ke složce `_generated`

V každém projektu (`Model`, `Entity`) je jen jedna (rootová) složka `_generated`. To umožňuje přehledné zobrazení pending changes (všechno generované lze snadno sbalit a přeskočit).
Soubory pro neexistující entity (odstraněné z modelu) se mažou. Mažou se i repositories, pro které neexistují entity (lze vypnout nastavením `SuppressRemovingRelicRepositories`).

### Poznámka k entitám pro vztah M:N

Pro entity reprezentující vztah M:N (entity mající jen složený primární klíč ze dvou sloupců a nic víc) se žádný kód negeneruje.