using System;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions
{
	/// <summary>
	/// Extension metódy pre konfiguráciu Model Extensions v kombinácii s Business Layerom.
	/// </summary>
	public static class BusinessLayerModelExtensionsExtensions
	{
		/// <summary>
		/// Zapne podporu pre extended properties na <see cref="IModelExtender"/> objektoch. Umožňuje automaticky spravovať extended properties pre objekty <see cref="IModelExtension"/> pomocou migrácii.
		/// </summary>
		/// <remarks>
		/// Je nutné odekorovať <see cref="IModelExtender"/> objekty pomocou atribútov dediacich z <see cref="ModelExtensionExtendedPropertiesAttribute"/>. Podporované sú len tie objekty v DB, na ktoré je možné pridať extended properties v SQL Serveri.
		/// </remarks>
		/// <returns>Inštancia <see cref="ModelExtensionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
		public static ModelExtensionsExtensionBuilder UseExtendedProperties(this ModelExtensionsExtensionBuilder optionsBuilder)
		{
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

			return new BusinessLayerModelExtensionsExtensionBuilder(optionsBuilder)
				.UseExtendedProperties();
		}

		/// <summary>
		/// Zapne podporu pre rozšírenia uložených procedúr podporovaných Business Layerom (resp. jeho generátorom). Jedná sa primárne o (polo)automatické generovanie extended properties pre uložené procedúry ako napr. MS_Description (z XML komentáru) alebo Attach (z Attach atribútu).
		/// </summary>
		/// <remarks>
		/// Pre podporu nastavenia MS_Description extended property je nutné zapnúť generovanie dokumentačného XML súboru z XML komentárov na projekte, kde sa <see cref="IModelExtension"/> uložených procedúr nachádzajú.
		/// </remarks>
		/// <returns>Inštancia <see cref="ModelExtensionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
		public static ModelExtensionsExtensionBuilder UseBusinessLayerStoredProcedures(this ModelExtensionsExtensionBuilder optionsBuilder)
		{
			Contract.Requires<ArgumentNullException>(optionsBuilder != null);

			return new BusinessLayerModelExtensionsExtensionBuilder(optionsBuilder)
				.UseBusinessLayerStoredProcedures();
		}
	}
}