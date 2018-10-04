using System;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.Metadata
{
	/// <summary>
	/// Indikuje, zda mají být generovány indexy k cizím klíčům tabulky.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public class GenerateIndexesAttribute : Attribute
	{
		/// <summary>
		/// Indikuje, zda mají být generovány indexy k cizím klíčům tabulky.
		/// </summary>
		public bool GenerateIndexes { get; set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public GenerateIndexesAttribute(bool generateIndexes = true)
		{
			GenerateIndexes = generateIndexes;
		}
	}
}