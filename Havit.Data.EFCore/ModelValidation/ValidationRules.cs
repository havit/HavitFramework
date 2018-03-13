namespace Havit.Data.EFCore.ModelValidation
{
	/// <summary>
	/// Pravidla, která model validator spustí.
	/// Ve výchozím stavu jsou zapnuta všechna pravidla.
	/// </summary>
	public class ValidationRules
	{
		/// <summary>
		/// Kontroluje, zda třída obsahuje právě jeden primární klíč.
		/// </summary>
		public bool CheckPrimaryKeyIsNotComposite { get; set; } = true;

		/// <summary>
		/// Kontroluje, zda třída obsahuje právě jeden primární klíč.
		/// </summary>
		public bool CheckPrimaryKeyNamingConvention { get; set; } = true;

		/// <summary>
		/// Kontroluje, zda je primární klíč typu System.Int32.
		/// </summary>
		public bool CheckPrimaryKeyType { get; set; } = true;

		/// <summary>
		/// Kontroluje, aby žádná vlastnost nekončila na "ID" (kapitálkami).
		/// </summary>
		public bool CheckIdNamingConvention { get; set; } = true;

		/// <summary>
		/// Kontroluje, zda mají všechny stringové vlastnosti uvedenu maximální délku.
		/// </summary>
		public bool CheckStringsHaveMaxLengths { get; set; } = true;

		/// <summary>
		/// Kontroluje, zda jsou použity pouze podporované nested types.
		/// </summary>
		public bool CheckSupportedNestedTypes { get; set; } = true;

		/// <summary>
		/// Kontroluje, zda mají navigační vlastnosti cizí klíč.
		/// </summary>
		public bool CheckNavigationPropertiesHaveForeignKeys { get; set; } = true;
		
		/// <summary>
		/// Kontroluje třídy, které mají Entry. Třídy, které mají vlastnost symbol, nesmí mít generovaný klíč a zároveň naopak třídy, které nemají vlastnost Symbo, musí mít generovaný klíč.
		/// </summary>
		public bool CheckSymbolVsPrimaryKeyForEntries { get; set; } = true;
	}
}