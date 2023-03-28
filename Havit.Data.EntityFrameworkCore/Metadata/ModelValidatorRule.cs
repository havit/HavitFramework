namespace Havit.Data.EntityFrameworkCore.Metadata;

/// <summary>
/// Pravidla pro validaci modelu.
/// Slouží k potlačení určitých pravidel modelu v <see cref="ModelValidation.ModelValidator"/>.
/// </summary>
public class ModelValidatorRule
{
	/// <summary>
	/// Kontroluje, zda všechny vlastnosti, jejichž název končí 'Id' jsou cizím klíčem.
	/// </summary>
	public static ModelValidatorRule OnlyForeignKeyPropertiesCanEndWithId { get; } = new ModelValidatorRule("SuppressModelValidatorRule_OnlyForeignKeyPropertiesCanEndWithId");

	internal string SuppressModelValidatorRuleAnnotationName { get; init; }

	private ModelValidatorRule(string suppressModelValidatorRuleAnnotationName)
	{
		SuppressModelValidatorRuleAnnotationName = suppressModelValidatorRuleAnnotationName;
	}
}
