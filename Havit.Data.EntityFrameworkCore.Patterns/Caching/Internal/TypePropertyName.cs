using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal
{
	/// <summary>
	/// Evidence typu entity a názvu vlastnosti pro použití jako klíč v Dictionary (<see cref="CollectionTargetTypeService"/>).
	/// </summary>
	public class TypePropertyName
	{
		/// <summary>
		/// Typ entity.
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// Vlastnost.
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public TypePropertyName(Type type, string propertyName)
		{
			Type = type;
			PropertyName = propertyName;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			TypePropertyName second = obj as TypePropertyName;
			return (second != null)
				&& (this.Type == second.Type)
				&& (this.PropertyName == second.PropertyName);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return Type.GetHashCode() ^ PropertyName.GetHashCode();
		}
	}
}
