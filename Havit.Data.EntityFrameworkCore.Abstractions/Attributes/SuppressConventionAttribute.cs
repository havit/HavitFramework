using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Attributes
{
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
