using System.Diagnostics.CodeAnalysis;

namespace Havit.Data.Entity.Validators;

/// <summary>
/// Pravidla, která model validator spustí.
/// Ve výchozím stavu jsou zapnuta všechna pravidla.
/// </summary>
[ExcludeFromCodeCoverage]
public class ValidationRules
{
	/// <summary>
	/// Kontroluje, zda třída obsahuje právě jeden primární klíč.
	/// </summary>
	public bool CheckPrimaryKeyIsNotComposite { get; set; } = true;

	/// <summary>
	/// Kontroluje, zda je primární klíč pojmenovaný "Id".
	/// </summary>
	public bool CheckPrimaryKeyName { get; set; } = true;

	/// <summary>
	/// Kontroluje, zda je primární klíč typu System.Int32.
	/// </summary>
	public bool CheckPrimaryKeyType { get; set; } = true;

	/// <summary>
	/// Kontroluje, aby žádná vlastnost nekončila na "ID" (kapitálkami).
	/// </summary>
	public bool CheckIdPascalCaseNamingConvention { get; set; } = true;

	/// <summary>
	/// Kontroluje, zda mají všechny stringové vlastnosti uvedenu maximální délku.
	/// </summary>
	public bool CheckStringsHaveMaxLengths { get; set; } = true;

	/// <summary>
	/// Kontroluje, zda jsou použity pouze podporované nested types.
	/// </summary>
	public bool CheckSupportedNestedTypes { get; set; } = true;

	/// <summary>
	/// Kontroluje třídy, které mají Entry. Třídy, které mají vlastnost symbol, nesmí mít generovaný klíč a zároveň naopak třídy, které nemají vlastnost Symbo, musí mít generovaný klíč.
	/// </summary>
	public bool CheckSymbolVsPrimaryKeyForEntries { get; set; } = true;

}