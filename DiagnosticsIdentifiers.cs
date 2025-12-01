namespace Havit.Diagnostics.Common
{
	/// <summary>
	/// Diagnostic identifiers.
	/// </summary>
	internal static class DiagnosticIdentifiers
	{
		/// <summary>
		/// ServiceType not found.
		/// </summary>
		public const string ServiceAttributeCannotDetermineServiceTypeId = "HFW1001"; // Category: Usage

		/// <summary>
		/// IEnumerable passed to AddFor* methods.
		/// </summary>
		public const string UnitOfWorkAddIEnumerableArgumentId = "HFW1002"; // Category: Usage

		/// <summary>
		/// IEnumerable of IEnumerable passed to AddRangeFor* methods.
		/// </summary>
		public const string UnitOfWorkAddRangeNestedCollectionId = "HFW1003"; // Category: Usage


	}
}
