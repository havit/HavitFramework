using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Attributes
{

	// TODO EF Core 3.0: Chyba návrhu - aby mohlo být použité, je potřeba typ potlačené konvence do atributu. Abychom jej mohli použít, museli bychom referencovat i Havit.Data.EntityFramework (a další) s EF Core *do modelu*!

	/// <summary>
	/// Slouží k označení konvenve jako potlačené.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
	public class SuppressConventionAttribute : Attribute
	{
		/// <summary>
		/// Potlačená konvence (resp. její typ).
		/// </summary>
		public Type ConventionTypeToSuppress { get; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public SuppressConventionAttribute(Type conventionTypeToSuppress)
		{
			ConventionTypeToSuppress = conventionTypeToSuppress;
		}
	}
}
