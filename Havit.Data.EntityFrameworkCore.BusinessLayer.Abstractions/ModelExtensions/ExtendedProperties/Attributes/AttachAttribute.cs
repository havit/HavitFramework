using System;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties.Attributes
{
	/// <summary>
	/// Atribút pre nastavenie Attach extended property na uloženej procedúre.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class AttachAttribute : Attribute
	{
		/// <summary>
		/// Názov uloženej procedúry.
		/// </summary>
		public string EntityName { get; }

		/// <summary>
		/// Konštruktor.
		/// </summary>
		public AttachAttribute(string entityName)
		{
			EntityName = entityName;
		}
	}
}