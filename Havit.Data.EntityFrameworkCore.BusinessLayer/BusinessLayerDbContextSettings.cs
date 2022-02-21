using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
    /// <summary>
    /// Konfigurácia <see cref="BusinessLayerDbContext"/>u, t.j. DbContextu pre Business Layer aplikácie (aka DB Migrations).
    ///
    /// <remarks>Úprava konfigurácie prebieha v metóde <see cref="BusinessLayerDbContext.CreateDbContextSettings"/>, ktorú je potrebné prepísať v aplikačnom DbContexte.</remarks>
    /// </summary>
	public class BusinessLayerDbContextSettings
	{
		/// <summary>
		/// Indikuje používání konvence CollectionExtendedPropertiesConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseCollectionExtendedPropertiesConvention { get; set; } = true;

        /// <summary>
        /// Indikuje používání konvence CollectionOrderIndexConvention.
        /// Výchozí hodnota je false (WIP).
        /// </summary>
        public bool UseCollectionOrderIndexConvention { get; set; } = false;

        /// <summary>
        /// Indikuje používání konvence ExtendedPropertiesConvention.
        /// Výchozí hodnota je true.
        /// </summary>
        public bool UseAttributeBasedExtendedPropertiesConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence DefaultValueAttributeConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseDefaultValueAttributeConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence DefaultValueSqlAttributeConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseDefaultValueSqlAttributeConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence ForeignKeysColumnNamesConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseForeignKeysColumnNamesConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence CharColumnTypeForCharPropertyConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseCharColumnTypeForCharPropertyConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence IndexForForeignKeysConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseForeignKeysIndexConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence IndexForLanguageUiCulturePropertyConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseLanguageUiCultureIndexConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence IndexForLocalizationTableConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool LocalizationTableIndexConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence LocalizationTablesParentEntitiesConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseLocalizationTablesParentEntitiesConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence NamespaceExtendedPropertyConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseNamespaceExtendedPropertyConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence PrefixedTablePrimaryKeysConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UsePrefixedTablePrimaryKeysConvention { get; set; } = true;

		/// <summary>
		/// Indikuje používání konvence XmlCommentsForDescriptionPropertyConvention.
		/// Výchozí hodnota je true.
		/// </summary>
		public bool UseXmlCommentsForDescriptionPropertyConvention { get; set; } = true;

        /// <summary>
        /// <see cref="Assembly"/>, ktorá obsahuje Model Extendery (objekty, ktoré reprezentujú rozšírenie dátového modelu napr. o uložené procedúry).
        /// </summary>
        /// <remarks>
        /// <para>Štandardne sa nastaví na assembly, v ktorej sa nachádza DbContext aplikácie (t.j. trieda, ktorá dedí od <see cref="BusinessLayerDbContext"/>).</para>
        /// </remarks>
        public Assembly ModelExtensionsAssembly { get; set; }
    }
}
