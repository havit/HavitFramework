## UnitOfWork

`IUnitOfWork` poskytuje metody:

* `Add[Range]ForInsert`
* `Add[Range]ForInsertAsync` - určeno pro použití [HiLo strategie](https://davecallan.com/how-to-use-hilo-with-entity-framework/) generování Id
* `Add[Range]ForUpdate`
* `Add[Range]ForDelete`
* `Commit[Async]`
* `RegisterAfterCommitAction`
* `Clear` - Umožňuje vyčistit ChangeTracker podkladového DbContextu.

#### Add[Range]ForDelete

Entity, které podporují soft delete jsou metodou `Add[Range]ForDelete` označeny jako smazané příznakem, nedojde k jejich fyzickému smazání, ale k aktualizaci (`UPDATE`).

Fyzické smazání entity podporující soft delete není aktuálně možné (kdo bude potřebovat, nechť se ozve, doplníme metodu `Add[Range]ForDestroy`).

### RegisterAfterCommitAction
Umožňuje přidat zvenku nějakou akci k provedení po commitu (odeslání emailu, smazání cache, atp.)
Umožnuje přidat jak synchronní akci tak asynchronní akci.
Asynchronní akce funguje pouze v asynchronním commitu, v případě registrace asynchronní akce a spuštění synchronního commitu dojde k vyhození výjimky.

#### Příklad

```csharp
private void ProcessPayment(Payment payment)
{
	...
	// vůbec nevíme, kde je unitOfWork.Commit(), ale víme, že po jeho spuštění dojde k odeslání notifikace
	unitOfWork.RegisterAfterCommitAction(() => SendNotification(payment));
	...	
}
```

### Koncept BeforeCommitProcessorů
`DbUnitOfWork` obsahuje koncept, který umožní při volání commitu spustit služby pro každou změněnou entitu ještě před uložením objektu. Je možné tak "na poslední chvíli" provést v entitách nějaké změny.

Pro implementaci nějakého vlastního `BeforeCommitProcessoru` je vhodné dědit z `BeforeCommitProcessor<TEntity>`, což pomůže vypořádat se s dvojicí metod `Run` a `RunAsync` v interface `IBeforeCommitProcessor<TEntity>`.

Službu je potřeba si zaregistrovat službu do DI containeru pod interface `IBeforeCommitProcessor<TEntity>`.

Metoda vrací hodnotu výčtu `ChangeTrackerImpact` a má pomoci `UnitOfWorku` s výkonovou optimalizací. Hodnota říká, zda změna provedená before commit processorem může ovlivnit changetracker tak, že je nutné jej spustit znovu (což je potřeba typicky jen při přidání nové entity).

#### Příklad

Viz např. implementace [`SetCreatedToInsertingEntitiesBeforeCommitProcessor`](https://dev.azure.com/havit/DEV/_git/002.HFW-HavitFramework?path=%2FHavit.Data.EntityFrameworkCore.Patterns%2FUnitOfWorks%2FBeforeCommitProcessors%2FSetCreatedToInsertingEntitiesBeforeCommitProcessor.cs&_a=contents&version=GBmaster).

```csharp
public class MyEntityBeforeCommitProcessor : BeforeCommitProcessor<MyEntity>
{
    public ChangeTrackerImpact Run(ChangeType changeType, MyEntity changingEntity)
    {
		if (changeType == ChangeType.Insert)
		{
			// do something
		}
		return ChangeTrackerImpact.NoImpact;
    }
}
```

#### SetCreatedToInsertingEntitiesBeforeCommitProcessor
Pro nově založené objekty, které mají vlastnost `Created` typu `DateTime` a v této vlastnosti je hodnota `default(DateTime)` nastaví aktuální čas (z `ITimeService`). Tj. automaticky nastavuje hodnotu `Created` entitám, které ji nastavenou nemají.

Je použit automaticky (díky registraci do DI containeru).

### Koncept EntityValidatorů

Před uložením objektů (a po spuštění `BeforeCommitProcessorů`) se spustí validátory entit, které umožňují kontrolovat jejich stav. Pokud je zjištěna nějaká validační chyba, je vyhozena výjimka typu `ValidationFailedException`.

Pro implementaci nějakého vlastního `EntityValidatoru` je třeba implementovat interface `IEntityValidator<TEntity>`. K implementaci je jediná metoda `Validate`, jež má na výstupu kolekci `IEnumerable<string>` - zjištěných chyb při validaci.

Dále je třeba službu zaregistrovat do DI containeru.

#### Příklad

```csharp
public class MyEntityEntityValidator : IEntityValidator<MyEntity>
{
	IEnumerable<string> Validate(ChangeType changeType, MyEntity changingEntity)
	{
		if (changingEntity.StartDate >= changingEntity.EndDate)
		{
			yield return "Počáteční datum musí předcházet koncovému datu.";
		}
	}
}
```

### IValidatableObject.Validate()
Jednou ze specifických možností implementace `EntityValidatoru` je `IValidatableObject.Validate()` přímo entitě.


```csharp 
public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
{
	if ((this.Parent == null) && (this.Id != (int)Project.Entry.Root))
	{
		yield return new ValidationResult($"Property {nameof(Parent)} is allowed to be null only for Root project.");
	}
	if ((this.Depth == 0) && (this.Id != (int)Project.Entry.Root))
	{
		yield return new ValidationResult($"Value 0 of {nameof(Depth)} property is allowed only for Root project.");
	}
}
```
Tyto validace lze pak do commit-sekvence zapojit zaregistrováním služby `ValidatableObjectEntityValidator` do DI containeru:

```csharp
services.AddSingleton<IEntityValidator<object>, ValidatableObjectEntityValidator>();
```

> ℹ️ ValidatableObjectEntityValidator nezajišťuje validace dle DataAnnotations atributů, jako jsou např. [Required], [MaxLength] apod.

### Pořadí akcí v commitu

Během commitu dochází postupně k těmto akcím:
* zavolání metody BeforeCommit
* spuštění BeforeCommitProcessorů
* spuštění EntityValidátorů
* uložení změn do databáze (DbContext.SaveChanges[Async]).
* zavolání metody AfterCommit (zajišťuje volání akcí registrovaných metodou RegisterAfterCommitAction)
