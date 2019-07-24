namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties.Attributes
{
	/// <summary>
	/// Určuje typ výsledku, který se má z metody volající stored proceduru vrátit.
	/// </summary>
	public enum StoredProcedureResultType
	{
		/// <summary>
		/// Nevrací žádný výsledek (void).
		/// </summary>
		None,
		
		/// <summary>
		/// Object vrací nejvýše jeden objekt nebo null, pokud není z uložené procedury vrácen žádný záznam.
		/// </summary>
		Object,

		/// <summary>
		/// Collection vrací kolekci objektů.
		/// </summary>
		Collection,

		/// <summary>
		/// DataTable vrací netypovou datovou tabulku.
		/// </summary>
		DataTable,

		/// <summary>
		/// DataSet vrací netypový DataSet.
		/// </summary>
		DataSet
	}
}