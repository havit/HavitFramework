using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers.Types;

/// <summary>
/// Reprezentuje záměr vytvořit property typu kolekce.
/// </summary>
public class CollectionProperty
{
	/// <summary>
	/// Název property typu kolekce.
	/// </summary>
	public string PropertyName { get; private set; }

	/// <summary>
	/// Parent prvků kolekce.
	/// </summary>
	public Table ParentTable { get; private set; }

	/// <summary>
	/// Typ prvků kolekce.
	/// </summary>
	public Table TargetTable { get; private set; }

	/// <summary>
	/// Je-li kolekce typu M:N je zde spojovací tabulka.
	/// </summary>
	public Table JoinTable { get; private set; }

	/// <summary>
	/// Sloupeček, který referencuje kolekci (určuje, že záznamy jsou prvky kolekce).
	/// </summary>
	public Column ReferenceColumn { get; private set; }

	/// <summary>
	/// Komentář k vlastnosti.
	/// </summary>
	public string Description { get; private set; }

	/// <summary>
	/// Určuje, zda se mají načíst hodnoty objektů kolekce (LoadAll).
	/// </summary>
	public bool LoadAll { get; private set; }

	/// <summary>
	/// Přístupový modifikátor pro getter property.
	/// </summary>
	public string PropertyAccessModifier { get; private set; }

	/// <summary>
	/// Řazení prvků kolekce po načtení z databáze.
	/// </summary>
	public string Sorting { get; private set; }

	/// <summary>
	/// Říká, zda se mají do kolekce načítat i smazané záznamy.
	/// </summary>
	public bool IncludeDeleted { get; private set; }

	/// <summary>
	/// Vrací true, pokud jde o kolekci typu 1:N.
	/// </summary>
	public bool IsOneToMany
	{
		get
		{
			return this.JoinTable == null;
		}
	}

	/// <summary>
	/// Vrací true, pokud jde o kolekci typu M:N.
	/// </summary>
	public bool IsManyToMany
	{
		get
		{
			return JoinTable != null;
		}
	}

	/// <summary>
	/// Režim klonování prvků kolekce.
	/// </summary>
	public CloneMode CloneMode
	{
		get
		{
			if (cloneMode == null)
			{
				string cloneModeValue = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(this.ParentTable), String.Format("Collection_{0}_CloneMode", this.PropertyName));
				if (string.IsNullOrEmpty(cloneModeValue))
				{
					// Historická poznámka: Dříve jsme jako default měli pro ManyToMany default Shallow, ale kód jsme pro pro Shallow negenerovali (bug).
					// Generování shallow klonování kolekcí bylo doplněno, ale pro zpětnou kompatibilitu nastavíme CloneMode na No (tak jsme se ke kolekcím chovali dosud i přes hodnotu "Shallow").
					cloneMode = CloneMode.No;
				}
				else
				{
					cloneMode = (CloneMode)Enum.Parse(typeof(CloneMode), cloneModeValue, true);
				}
			}
			return cloneMode.Value;
		}
	}
	private CloneMode? cloneMode;

	/// <summary>
	/// Vytvoří inicializovanou instanci.
	/// </summary>
	public CollectionProperty(Table parentTable, string propertyName, Table joinTable, Table targetTable, Column referenceColumn, string description, bool loadAll, string propertyAccessModifier, string sorting, bool includeDeleted)
	{
		this.ParentTable = parentTable;
		this.PropertyName = propertyName;
		this.JoinTable = joinTable;
		this.TargetTable = targetTable;
		this.ReferenceColumn = referenceColumn;
		this.Description = description;
		this.LoadAll = loadAll;
		this.PropertyAccessModifier = propertyAccessModifier;
		this.Sorting = sorting;
		this.IncludeDeleted = includeDeleted;
	}
}
