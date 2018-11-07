using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	/// <summary>
	/// ExtendedProperties pro CloneMode.
	/// </summary>
	public class CloneModeAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Režim klonování vlasntostí.
		/// </summary>
		public CloneMode CloneMode { get; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public CloneModeAttribute(CloneMode cloneMode)
		{
			CloneMode = cloneMode;
		}

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
		{
			bool isCollection = (memberInfo is PropertyInfo propertyInfo)
				&& propertyInfo.PropertyType.GetInterfaces().Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>));

			return isCollection
				// extended property pro kolekce jsou generovány nad tabulkou
				? new Dictionary<string, string>().AddIfNotDefault($"Collection_{memberInfo.Name}_CloneMode", CloneMode, CloneMode.No)
				// extended property pro property jsou generovány nad sloupcem
				: new Dictionary<string, string>().AddIfNotDefault($"CloneMode", CloneMode, CloneMode.Shallow);
		}
	}
}
