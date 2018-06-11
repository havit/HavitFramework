namespace Havit.Business.BusinessLayerGenerator.Helpers.Types
{
	/// <summary>
	/// Určuje množství dat v resultu.
	/// </summary>
	public enum StoreProcedureResultType
	{
		/// <summary>
		/// Žádná návratová hodnota se nevrací.
		/// </summary>
		None,

		/// <summary>
		/// Vrací se objekt.
		/// </summary>
		Object,

		/// <summary>
		/// Vrací se kolekce objektů.
		/// </summary>
		Collection,

		/// <summary>
		/// Vrací se DataTable.
		/// </summary>
		DataTable,

		/// <summary>
		/// Vrací se DataSet.
		/// </summary>
		DataSet
	}
}
