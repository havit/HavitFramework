## Kolekce s filtrováním smazaných záznamů

Bohužel není možné **načíst** jen nesmazané záznamy. Můžeme však načíst do paměti všechny záznamy a používat jen ty nesmazané,
například vytvořením dvou kolekcí - persistentní (se všemi objekty) a nepersistentní (počítaná, filtruje jen nesmazané záznamy s persistentní kolekce.

V následujících ukázkách budeme pracovat s třídou `Child`, kterou lze příznakem označit za smazanou a s třídou `Master` mající kolekci `Children` objektů `Child`.

Pro implementaci potřebujeme zajistit:

* Použití kolekcí v modelu
* Mapování vlastností (kolekcí) v EF
* Kolekce filtrující smazané záznamy

**Filtrované kolekce není možné používat v queries (Where, OrderBy) ani v Include. V DataLoaderu je možné filtrované kolekce použít pro načtení záznamů.**

### Použití kolekcí v modelu

```csharp
public class Child
{
	public int Id { get; set; }
	public int MasterId { get; set; }
	public Master Master { get; set; }
	public DateTime? Deleted { get; set; }
}
 
public class Master
{
	public int Id { get; set; }

	public ICollection<Child> Children { get; } // nepersistentní
	public IList<Child> ChildrenIncludingDeleted { get; } = new List<Child>(); // persistentní

	public Master()
	{
		// kolekce children je počítanou kolekcí
		Children = new FilteringCollection<Child>(ChildrenIncludingDeleted, child => child.Deleted == null);
	}
}
```

### Mapování vlastností (kolekcí) v EF

```csharp
public class MasterConfiguration : IEntityTypeConfiguration<Master>
{
    public void Configure(EntityTypeBuilder<Master> builder)
    {
        builder.Ignore(c => c.Children);
        builder.HasMany(c => c.ChildrenIncludingDeleted);
    }
}
```

### Kolekce filtrující smazané záznamy

Viz `Havit.Model.Collections.Generic.FilteringCollection<T>` - [zdrojáky](https://havit.visualstudio.com/DEV/_git/002.HFW-HavitFramework?path=%2FHavit.Model%2FCollections%2FGeneric%2FFilteringCollection.cs&version=GBmaster). Kolekce je v nuget balíčku `Havit.Model`.

```csharp
public class FilteringCollection<T> : ICollection<T>
{
	private readonly ICollection<T> source;
	private readonly Func<T, bool> filter;

	public FilteringCollection(ICollection<T> source, Func<T, bool> filter)
	{
		this.source = source;
		this.filter = filter;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return source.Where(filter).GetEnumerator();
	}

    ...
}
```

